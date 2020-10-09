
using NetworkWPF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Text.Json;
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

    public class LandlordsGameMode : GameModeInterface
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
                User user = users[i];
                Player player = players[i];

                string data = JsonSerializer.Serialize(player.GetCards());
                user.Send(new Package(Package.OPT, "GameCallPage", "onStart", data));
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



        //先检测此回合是否为本地玩家进行操作，如果是则将按键对应的指令发送给服务器
        //指令同步，对于指令类游戏，不需要在本地运行任何游戏性操作，只需要相关的游戏状态，比如游戏是否结束，手牌和数量，赢家和输家
        public void ProcessInput(string data, User sender)
        {
            inputPoint = -1;

            switch (mGameSession)
            {
                case GameSession.CALL:

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
                    break;
                case GameSession.PLAYING:
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
                    break;
                case GameSession.FINISH:

                    break;
                default:
                    break;
            }

        }
        //不需要在本地处理各种操作，而是放到服务器，服务器告诉谁应该可以做什么

        public void UpdateGame(string data, User sender)
        {
            switch (mGameSession)
            {

                case GameSession.PREPARE:

                    break;
                case GameSession.START:
                    ReStart(sender);
                    
                    break;
                case GameSession.CALL:
                    if (inputPoint == -1)
                    {
                        break;
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
                    break;
                case GameSession.FINISH:
                    break;

                default:
                    break;
            }
        }

        //服务器不需要处理到屏幕的输出，只需要通知客户端状态，客户端根据状态进行相应的显示
        public string GenerateOutput(string data, User sender)
        {
            return "";
        }
    }
}
