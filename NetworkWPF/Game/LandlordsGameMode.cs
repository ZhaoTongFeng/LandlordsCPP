
using NetworkWPF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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

    public class LandlordsGameMode : GameModeInterface,INetwork
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


            for (int i = 0; i < 3; i++){
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

            mHander.HandCards(ref players, ref mDarkCards);

            //将牌发送到客户端

            //房间中的三个用户，和players一一对应
            List<User> users = sender.room.users;


            

            for(int i = 0; i < users.Count; i++)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                User user = users[i];
                Player player = players[i];

                //手牌
                string cardsbuf = JsonSerializer.Serialize(player.GetCards());
                data.Add("cards", cardsbuf);

                int[] count_cards = new int[2];
                //另外两家手牌数量
                for(int j = 1; j <= 2; j++)
                {
                    User user_next = users[(i+j)%3];
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
                    data.Add("this","1");

                    //开启倒计时
                    StartTimer();

                    //初始化计时器，每次操作之后都会初始化计时器
                    tick = 5;
                }


                user.Send(new Package(Package.OPT, "GameCallPage", "onStart", JsonSerializer.Serialize(data)));
            }
        }
        private Room mRoom;

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


        ////////////////////////////////////////
        //结算阶段
        ////////////////////////////////////////


        /// <summary>
        /// 结算是否结束
        /// </summary>
        bool isFinish = false;

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
            throw new NotImplementedException();
        }


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

        private void Call(string data, User sender)
        {
            if (mGameSession != GameSession.CALL)
            {
                return;   
            }
            //ProcessInput
            inputPoint = -1;
            if (mCallState == CallState.NO)
            {
                if (data.Equals("不叫"))
                {
                    inputPoint = 0;
                }
                else if (data.Equals("叫地主"))
                {
                    mCallState = CallState.CALL;
                    inputPoint = 1;
                }
            }
            else
            {
                if (data.Equals("不叫"))
                {
                    inputPoint = 0;
                }
                else if (data.Equals("抢地主"))
                {
                    mCallState = CallState.Rob;
                    inputPoint = 2;
                }
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
            if (isFinishCall)
            {
                mGameSession = GameSession.PLAYING;
                mCurPlayerIndex = mLandlordsIndex;
                CardsBuf cardsBuf = GetCurPlayer().GetCards();
                mDarkCards.Pop(ref cardsBuf, 3);
                cardsBuf.SortRank(false);

                GetCurPlayer().mTeamID = 1;
                NextPlayer();
                GetCurPlayer().mTeamID = 2;
                NextPlayer();
                GetCurPlayer().mTeamID = 2;
                NextPlayer();
            }
        }

        private void HandOut(string data, User sender)
        {
            if (mGameSession != GameSession.PREPARE)
            {
                return;
            }
            inputPoint = -1;
            if (!mLastCards.IsEmpty())
            {
                if (data.Equals("不要"))
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
            }
            CardsBuf cards = JsonSerializer.Deserialize<CardsBuf>(data);
            if (cards != null)
            {
                CardsBuf ca = GetCurPlayer().GetCards();
                if (ca.OutCards(ref mPreCards, ref mLastCards))
                {
                    mLastCards.Copy(mPreCards);
                    mPreCards.SetEmpty();
                    NextPlayer();
                    mMissCount = 0;
                }
                if (ca.IsEmpty())
                {
                    mGameSession = GameSession.FINISH;
                }
            }
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


    }
}
