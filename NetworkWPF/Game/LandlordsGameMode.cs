
using NetworkWPF;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Text.Json;
using System.Threading;

namespace LandlordsCS
{

    public enum GameSession
    {
        //准备阶段
        PREPARE,
        //开始阶段
        START,
        //叫牌阶段
        CALL,
        //游戏阶段
        PLAYING,
        //结束阶段
        FINISH,
        //阶段总数
        NUM_SESSION
    }
    //叫牌状态
    public enum CallState
    {
        NO,
        CALL,
        Rob
    }

    public class LandlordsGameMode : GameModeInterface, INetwork
    {


        /// <summary>
        /// 游戏阶段
        /// </summary>
        GameSession mGameSession { get; set; }

        /// <summary>
        /// 当前用户下标
        /// </summary>
        int mCurPlayerIndex { get; set; }

        /// <summary>
        /// 游戏玩家
        /// </summary>
        List<Player> players;

        List<User> users;

        private Room mRoom;



        /// <summary>
        /// 下一个玩家
        /// </summary>
        void NextPlayer() { mCurPlayerIndex = (mCurPlayerIndex + 1) % 3; }

        /// <summary>
        /// 获取当前游戏玩家
        /// </summary>
        /// <returns></returns>
        Player GetCurPlayer() { return players[mCurPlayerIndex]; }

        /// <summary>
        /// 荷官
        /// </summary>
        LandlordsHander mHander;



        public void ProcessData(Package package, User sender)
        {
            if (!package.clsName.Equals("LandlordsGameMode"))
            {
                return;
            }
            switch (package.funName)
            {
                case "Call":
                    Call(package.data, sender);
                    break;
                case "HandOut":
                    HandOut(package.data, sender);
                    break;

                default:
                    break;
            }
        }
        ////////////////////////////////////////
        //开始阶段
        ////////////////////////////////////////
        public LandlordsGameMode()
        {
            players = new List<Player>(3);
            mHander = new LandlordsHander(54);
            mDarkCards = new CardsBuf(3);

            mLastCards = new CardsBuf(20);
            mPreCards = new CardsBuf(20);

            for (int i = 0; i < 3; i++)
            {
                players.Add(new Player("PLAYER-" + i, 1000));
                CardsBuf cards = new CardsBuf(20);
                players[i].AddCardsBuf(ref cards);
            }
        }

        public void ReStart(User sender)
        {
            mRoom = sender.room;

            mCurPlayerIndex = 0;


            mGameSession = GameSession.CALL;
            mDarkCards.MakeEmpty();
            mCallState = CallState.NO;


            for (int i = 0; i < 3; i++)
            {
                mCallArr[i] = 0;
            }


            mCallArrCount = 0;
            mLandlordsIndex = -1;
            numCall = 0;
            inputPoint = -1;


            gate = false;
            mMissCount = 0;


            mLastCards.MakeEmpty();
            mPreCards.MakeEmpty();

            mCurPlayerIndex = mHander.HandCards(ref players, ref mDarkCards);

            //将牌发送到客户端

            //房间中的三个用户，和players一一对应
            users = sender.room.users;




            for (int i = 0; i < users.Count; i++)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                User user = users[i];
                Player player = players[i];

                //手牌
                string cardsbuf = JsonSerializer.Serialize(player.GetCards());
                data.Add("cards", cardsbuf);
                
                //另外两家手牌数量
                int[] count_cards = new int[2];
                for (int j = 1; j <= 2; j++)
                {
                    User user_next = users[(i + j) % 3];
                    Player player_next = players[(i + j) % 3];
                    count_cards[j - 1] = player_next.GetCards().GetSize();
                }
                data.Add("count_cards", JsonSerializer.Serialize(count_cards));

                //当前用户在房间中的位置
                data.Add("indexInRoom", i.ToString());

                //当前正在执行操作的玩家下标
                data.Add("curOptIndex", mCurPlayerIndex.ToString());

                //当前游戏阶段
                data.Add("GameSession", mGameSession.ToString());

                //当前操作玩家
                if (mCurPlayerIndex == i)
                {
                    //这个玩家，应该用什么模式显示按钮
                    //是叫牌还是出牌由当前游戏阶段决定
                    data.Add("this", "1");

                    //开启倒计时
                    StartTimer();

                    //初始化计时器，每次操作之后都会初始化计时器
                    tick = 5;
                }


                user.Send(new Package(Package.OPT, "GameCallPage", "onStart", JsonSerializer.Serialize(data)));
            }
        }

        public void StartTimer()
        {
            Thread thread = new Thread(Timer);
            thread.IsBackground = true;
            thread.Start();
        }

        private long lastTime = 0;

        private int tick = 0;

        public void Timer()
        {
            while (Server.isRunning)
            {
                long cur = Server.GetMilTime();

                if (cur - lastTime > 1000)
                {
                    lastTime = cur;

                    tick--;

                    mRoom.SendToAllClient(new Package(Package.OPT, "GameCallPage", "onTimer", tick.ToString()));


                }
            }
        }


        ////////////////////////////////////////
        //叫牌阶段
        ////////////////////////////////////////

        /// <summary>
        /// 底牌
        /// </summary>
        CardsBuf mDarkCards;

        /// <summary>
        /// 叫牌状态
        /// </summary>
        CallState mCallState { get; set; }

        /// <summary>
        /// 参与叫牌的人数及其下标，第一个人为叫地主的人
        /// </summary>
        int[] mCallArr = new int[3];
        int mCallArrCount = 0;

        /// <summary>
        /// 地主下标
        /// </summary>
        int mLandlordsIndex = -1;

        /// <summary>
        /// 叫牌次数
        /// 小于等于四次
        /// </summary>
        int numCall = 0;

        /// <summary>
        /// 叫牌点数
        /// </summary>
        int inputPoint = -1;




        private void Call(string data, User sender)
        {
            if (mGameSession != GameSession.CALL)
            {
                return;
            }
            Dictionary<string, string> outData = new Dictionary<string, string>();



            //ProcessInput
            inputPoint = -1;
            if (mCallState == CallState.NO)
            {
                if (data.Equals("不叫"))
                {
                    inputPoint = 0;
                    outData.Add("callState", "no");
                }
                else if (data.Equals("叫地主"))
                {
                    mCallState = CallState.CALL;
                    inputPoint = 1;
                    outData.Add("callState", "call");
                }
            }
            else
            {
                if (data.Equals("不叫"))
                {
                    inputPoint = 0;
                    outData.Add("callState", "no");
                }
                else if (data.Equals("抢地主"))
                {
                    mCallState = CallState.Rob;
                    inputPoint = 2;
                    outData.Add("callState", "rob");
                }

            }

            int showState = 0;
            if (mCallState == CallState.NO)
            {
                showState = 1;
            }
            else
            {
                showState = 2;
            }

            //Update
            if (inputPoint == -1)
            {
                return;
            }



            numCall++;
            bool isFinishCall = false;
            if (numCall <= 2)
            {
                if (inputPoint == 1)
                {
                    mLandlordsIndex = mCurPlayerIndex;
                }
                if (inputPoint != 0)
                {
                    mCallArr[mCallArrCount++] = mCurPlayerIndex;
                }
                NextPlayer();
            }
            else if (numCall == 3)
            {
                if (inputPoint == 1)
                {
                    mLandlordsIndex = mCurPlayerIndex;
                }
                if (inputPoint != 0)
                {
                    mCallArr[mCallArrCount++] = mCurPlayerIndex;
                }
                if (mCallArrCount == 0)
                {
                    mGameSession = GameSession.START;
                }
                else if (mCallArrCount == 1)
                {
                    mLandlordsIndex = mCallArr[0];
                    isFinishCall = true;
                }
                else
                {
                    mCurPlayerIndex = mLandlordsIndex;
                }
            }
            else if (numCall == 4)
            {
                isFinishCall = true;
                if (inputPoint == 0)
                {
                    mLandlordsIndex = mCallArr[1];
                }
                else if (inputPoint == 2)
                {
                    mLandlordsIndex = mCallArr[0];
                }
            }

            outData.Add("curOptIndex", mCurPlayerIndex.ToString());

            if (isFinishCall)
            {
                mGameSession = GameSession.PLAYING;
                mCurPlayerIndex = mLandlordsIndex;

                CardsBuf cardsBuf = GetCurPlayer().GetCards();
                mDarkCards.Pop(ref cardsBuf, 3);
                cardsBuf.SortRank(false);

                //重新发送有序的手牌
                outData.Add("newCards", JsonSerializer.Serialize(cardsBuf));

                GetCurPlayer().mTeamID = 1;
                NextPlayer();
                GetCurPlayer().mTeamID = 2;
                NextPlayer();
                GetCurPlayer().mTeamID = 2;
                NextPlayer();

                //地主下标
                outData.Add("LandlordsIndex", mLandlordsIndex.ToString());
                //三张底牌
                outData.Add("DarkCards", JsonSerializer.Serialize(mDarkCards));

                //按键模式，只能出牌
                showState = 1;
                outData.Add("showState", showState.ToString());



                mRoom.SendToAllClient(new Package(Package.OPT, "GameCallPage", "onCallFinish", JsonSerializer.Serialize(outData)));
            }
            else
            {

                outData.Add("showState", showState.ToString());
                mRoom.SendToAllClient(new Package(Package.OPT, "GameCallPage", "onCall", JsonSerializer.Serialize(outData)));
            }

        }

        ////////////////////////////////////////
        //出牌阶段
        ////////////////////////////////////////

        //将选中的牌放到一个列表中，检测是否符合牌型，并记录牌型
        bool gate = false;

        //两家不要牌则判定为新回合开始
        int mMissCount = 0;

        //上一出牌
        CardsBuf mLastCards;

        //准备出的牌
        //将选择的牌放到这儿
        CardsBuf mPreCards;








        private void HandOut(string data, User sender)
        {
            if (mGameSession != GameSession.PLAYING)
            {
                return;
            }

            Dictionary<string, string> outData = new Dictionary<string, string>();
            //出牌是否成功默认为0成功
            int isSucc = 0;
            

            //客户端传入：打出的牌的下标
            List<int> indexs = JsonSerializer.Deserialize<List<int>>(data);
            Player handOutPlayer = GetCurPlayer();
            CardsBuf ca = handOutPlayer.GetCards();

            if (indexs.Count==0)
            {
                //如果上一出牌为空则允许不要
                if (!mLastCards.IsEmpty())
                {
                    NextPlayer();
                    mPreCards.MakeEmpty();
                    mMissCount++;
                    if (mMissCount == 2)
                    {
                        mLastCards.MakeEmpty();
                        mMissCount = 0;
                    }
                }
                else
                {
                    Server.Log("ERR:不要失败，上一出牌不为空");
                    //BUG
                    isSucc = 1;
                }
            }
            else
            {
                //根据下标从玩家牌堆中取出手牌放到准备牌堆
                mPreCards.MakeEmpty();
                foreach (int item in indexs)
                {
                    mPreCards.Push(ca.buf[item]);
                }

                //出牌
                if (ca.OutCards(ref mPreCards, ref mLastCards))
                {
                    mLastCards.Copy(mPreCards);
                    mPreCards.SetEmpty();
                    NextPlayer();
                    mMissCount = 0;
                    Server.Log("出牌成功");
                }
                else
                {

                    //返回_1：出牌失败，返回失败原因
                    isSucc = 2;
                    Server.Log("出牌失败");

                    outData.Add("isSucc", isSucc.ToString());
                    mRoom.SendToAllClient(new Package(Package.OPT, "GameCallPage", "onHandOut", JsonSerializer.Serialize(outData)));
                    return;
                }
            }


            //返回_2：出牌成功

            //打出的牌，长度可能为0
            outData.Add("LastCards", JsonSerializer.Serialize(mLastCards));

            //三个玩家手牌数量，无差别发送给客户端，由客户端自己处理

            int[] count_cards = new int[3];
            for (int j = 0; j < 3; j++)
            {
                count_cards[j] = players[j].GetCards().GetSize();
            }
            outData.Add("count_cards", JsonSerializer.Serialize(count_cards));
            outData.Add("curOptIndex", mCurPlayerIndex.ToString());
            outData.Add("isSucc", isSucc.ToString());
            int showState = mLastCards.IsEmpty() ? 1 : 2;
            outData.Add("showState", showState.ToString());
            outData.Add("CurrentCards", JsonSerializer.Serialize(ca));

            mRoom.SendToAllClient(new Package(Package.OPT, "GameCallPage", "onHandOut", JsonSerializer.Serialize(outData)));


            if (ca.IsEmpty())
            {
                //游戏结束
                mGameSession = GameSession.FINISH;

                //胜利队伍
                winTeam = handOutPlayer.mTeamID;


                Dictionary<string, string> outFinishData = new Dictionary<string, string>();
                //奖罚金额(arr[0]地主积分，arr[1]农民)
                mHander.Settle(players, users, winTeam, mLandlordsIndex, out int[] points);
                outFinishData.Add("points",JsonSerializer.Serialize(points));
                //int winTeam = arr[5];
                //int landlordIndex = arr[6];

                //三个用户名
                string[] userNames = new string[3];
                userNames[0] = users[mLandlordsIndex].name;
                userNames[1] = users[(mLandlordsIndex+1)%3].name;
                userNames[2] = users[(mLandlordsIndex+2)%3].name;
                outFinishData.Add("userNames", JsonSerializer.Serialize(userNames));


                for (int i = 0; i < users.Count; i++)
                {
                    User user = users[i];
                    if (players[i].mTeamID == winTeam)
                    {
                        bool isWin = true;
                        outFinishData.Add("isWin", isWin.ToString());
                    }
                    else
                    {
                        bool isWin = false;
                        outFinishData.Add("isWin", isWin.ToString());
                    }
                    if (i == mLandlordsIndex)
                    {
                        bool isLand = true;
                        outFinishData.Add("isLand", isLand.ToString());
                    }
                    else
                    {
                        bool isLand = false;
                        outFinishData.Add("isLand", isLand.ToString());
                    }
                    user.Send(new Package(Package.OPT, "GameCallPage", "onGameFinish", JsonSerializer.Serialize(outFinishData)));
                    user.Send(new Package(Package.OPT, "GameResultPage", "onGameFinish", JsonSerializer.Serialize(outFinishData)));
                }
            }
        }


        ////////////////////////////////////////
        //结算阶段
        ////////////////////////////////////////

        /// <summary>
        /// 胜利的队伍
        /// </summary>
        int winTeam = 0;

        /// <summary>
        /// 结算结果
        /// </summary>
        string mSettleContent;

        /// <summary>
        /// 结算
        /// </summary>
        public void Settle()
        {

        }




        //////////////////////////////////////////////////////
        /// 客户端传入一个输入，就处理一个，相当于之前的单步调试
        /// 下面的几个函数全部用不上了，这里只处理游戏逻辑即可
        /// 只剩发牌，叫牌，出牌
        //////////////////////////////////////////////////////

        //先检测此回合是否为本地玩家进行操作，如果是则将按键对应的指令发送给服务器
        //指令同步，对于指令类游戏，不需要在本地运行任何游戏性操作，只需要相关的游戏状态，比如游戏是否结束，手牌和数量，赢家和输家
        public void ProcessInput(string data, User sender)
        {
        }
        //不需要在本地处理各种操作，而是放到服务器，服务器告诉谁应该可以做什么
        public void UpdateGame(string data, User sender)
        {

        }
        //服务器不需要处理到屏幕的输出，只需要通知客户端状态，客户端根据状态进行相应的显示
        public string GenerateOutput(string data, User sender)
        {
            return "";
        }

        private void PrintCards()
        {

            for (int i = 0; i < 3; i++)
            {
                Server.Log(players[i].GetCards().GetPrintName());
            }
        }


    }
}
