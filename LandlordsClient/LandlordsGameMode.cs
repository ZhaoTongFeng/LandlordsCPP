using LandlordsClient;
using NetworkTCP;
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

        public bool isActivited = false;

        GameSession mGameSession = GameSession.PREPARE;

        int mCurPlayerIndex;
        List<Player> players;

        void NextPlayer() { mCurPlayerIndex = (mCurPlayerIndex + 1) % 3; }
        Player GetCurPlayer() { return players[mCurPlayerIndex]; }

        LandlordsHander mHander;
        ////////////////////////////////////
        //底牌
        CardsBuf mDarkCards;
            

        /////////////////////////////////////
        //叫牌状态
        CallState mCallState = CallState.NO;

        //参与叫牌的人数及其下标，第一个人为叫地主的人
        int[] mCallArr = new int[3];
        int mCallArrCount = 0;

        //标记叫地主的玩家
        int mLandlordsIndex = -1;

        //叫牌次数，小于等于四次
        int numCall = 0;

        //叫牌点数
        int inputPoint = -1;
        ////////////////////////////////////////
        //出牌
        //将选中的牌放到一个列表中，检测是否符合牌型，并记录牌型
        bool gate = false;

        //两家不要牌则判定为新回合开始
        int mMissCount = 0;

        //上一出牌
        CardsBuf mLastCards;

        //准备出的牌
        //将选择的牌放到这儿
        CardsBuf mPreCards;


        ///////////////
        //结算

        //结算是否结束
        bool isFinish = false;

        //胜利的队伍
        int winTeam = 0;

        //结算结果
        string mSettleContent;

        public void ReStart()
        {
            mGameSession = GameSession.CALL;
            mCurPlayerIndex = 0;
            mDarkCards.MakeEmpty();
            mCallState = CallState.NO;
            for(int i = 0; i < 3; i++)
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
        }

        public void Settle()
        {
            throw new NotImplementedException();
        }


        public LandlordsGameMode()
        {
            mGameSession = GameSession.PREPARE;
            mCurPlayerIndex = 0;
            players = new List<Player>(3);
            mHander = new LandlordsHander(54);
            mDarkCards = new CardsBuf(3);

            mLastCards = new CardsBuf(20);
            mPreCards = new CardsBuf(20);

            for(int i = 0; i < 3; i++)
            {
                players.Add(new Player("PLAYER-" + i, 1000));
                CardsBuf cards = new CardsBuf(20);
                players[i].AddCardsBuf(ref cards);
            }
        }

        public void ProcessNetworkPackage(Package pg)
        {

        }

        //先检测此回合是否为本地玩家进行操作，如果是则将按键对应的指令发送给服务器
        //指令同步，对于指令类游戏，不需要在本地运行任何游戏性操作，只需要相关的游戏状态，比如游戏是否结束，手牌和数量，赢家和输家

        public void ProcessInput()
        {
            inputPoint = -1;
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo info = Console.ReadKey(true);
                if (info.Key == ConsoleKey.R)
                {
                    mGameSession = GameSession.START;
                }
                switch (mGameSession)
                {
                    case GameSession.PREPARE:
                        if (info.Key == ConsoleKey.S)
                        {
                            mGameSession = GameSession.START;
                        }
                        break;
                    case GameSession.START:
                        break;
                    case GameSession.CALL:
                        if (mCallState == CallState.NO)
                        {
                            if (info.Key == ConsoleKey.Q)
                            {
                                inputPoint = 0;
                            }
                            else if (info.Key == ConsoleKey.W)
                            {
                                mCallState = CallState.CALL;
                                inputPoint = 1;
                            }
                        }
                        else
                        {
                            if (info.Key == ConsoleKey.Q)
                            {
                                inputPoint = 0;
                            }
                            else if (info.Key == ConsoleKey.E)
                            {
                                mCallState = CallState.Rob;
                                inputPoint = 2;
                            }
                        }
                        break;
                    case GameSession.PLAYING:
                        if (!mLastCards.IsEmpty())
                        {
                            if (info.Key == ConsoleKey.N)
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
                        if (info.Key == ConsoleKey.B)
                        {
                            gate = true;
                        }
                        if (info.Key == ConsoleKey.C)
                        {
                            mPreCards.MakeEmpty();
                        }
                        if (info.Key == ConsoleKey.H)
                        {
                            CardsBuf ca = GetCurPlayer().GetCards();
                            if(ca.OutCards(ref mPreCards, ref mLastCards))
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
        }
        //不需要在本地处理各种操作，而是放到服务器，服务器告诉谁应该可以做什么
        public void UpdateGame(float deltaTime)
        {
            switch (mGameSession)
            {
                case GameSession.PREPARE:
                    break;
                case GameSession.START:
                    ReStart();
                    mHander.HandCards(ref players, ref mDarkCards);
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
                    }else if (numCall == 3)
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
                        }else if (mCallArrCount == 1)
                        {
                            mLandlordsIndex = mCallArr[0];
                            isFinishCall = true;
                        }
                        else
                        {
                            mCurPlayerIndex = mLandlordsIndex;
                        }
                    }else if (numCall == 4)
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
                case GameSession.PLAYING:
                    if (!gate)
                    {
                        break;
                    }
                    CardsBuf ca = GetCurPlayer().GetCards();
                    string mCardsBuffer = Console.ReadLine();
                    string[] splits = mCardsBuffer.Split(' ');
                    foreach (string item in splits)
                    {
                        mPreCards.Push(ca.buf[int.Parse(item)]);
                    }
                    gate = false;
                    break;
                case GameSession.FINISH:
                    break;
                case GameSession.NUM_SESSION:
                    break;
                default:
                    break;
            }
        }
        //服务器不需要处理到屏幕的输出，只需要通知客户端状态，客户端根据状态进行相应的显示
        public string GenerateOutput()
        {
            string str = "";

            str += "*****************************\n";
            str += "牌堆牌数：" + mHander.GetSize() + "\n";
            str += "牌堆牌面：";
            str += mHander.GetPrintName();

            str += "三张底牌：";
            str += mDarkCards.GetPrintName();


            CardsBuf cb;
            for (int i = 0; i < 3; i++)
            {
                cb = players[i].GetCards();

                str += "玩家名称：" + players[i].mName + "\t";
                str += "积分：" + players[i].mBalance + "\t";
                str += "手牌数量：" + cb.GetSize() + "\t";
                str += "队伍：";
                if (players[i].mTeamID == 1)
                {
                    str += "地主\n";
                }
                else if (players[i].mTeamID == 2)
                {
                    str += "农民\n";
                }
                else
                {
                    str += "未知\n";
                }
                str += "手牌：";
                str += cb.GetPrintName();

            }
            str += "*****************************\n";
            
            cb = GetCurPlayer().GetCards();

            if (mLandlordsIndex != -1)
            {
                str += "地主名称：" + players[mLandlordsIndex].mName + "\n";
            }
            str += "当前玩家：" + GetCurPlayer().mName + "\n";
            str += "手牌数量：" + cb.GetSize() + "\n";
            str += "玩家手牌：";
            str += cb.GetPrintName();
            switch (mGameSession)
            {
                case GameSession.PREPARE:
                    str += "准备阶段\n";
                    str += "按S开始";
                    break;
                case GameSession.CALL:
                    str += "叫牌阶段\n";
                    if (mCallState == CallState.NO)
                    {
                        str += "q\tw\n";
                        str += "不要\t叫地主\n";
                    }
                    else
                    {
                        str += "q\te\n";
                        str += "不要\t抢地主\n";
                    }
                    break;
                case GameSession.PLAYING:

                    str += "出牌阶段\n\n";

                    str += "上一次出牌：";
                    if (!mLastCards.IsEmpty())
                    {
                        str += mLastCards.GetPrintName();
                        str += "牌型：" + mLastCards.GetCardsModeName() + "\n";
                    }
                    else
                    {
                        str += "无\n";
                    }
                    str += "\n";

                    str += "玩家操作：\n";

                    if (mLastCards.IsEmpty())
                    {
                        str += "B：选择要出的牌\tH：确定出牌\tC：清除出牌\n";
                    }
                    else
                    {
                        str += "N：不要\tB：选牌\tH：出牌\tC：清除\n";
                    }

                    str += "牌型：" + mPreCards.GetCardsModeName() + " ";
                    str += "已选：\n";
                    str += mPreCards.GetPrintName();




                    break;
                case GameSession.FINISH:
                    str += "结算阶段\n";
                    if (winTeam == 1)
                    {
                        str += "地主胜利！\n";
                    }
                    else
                    {
                        str += "农民胜利！\n";
                    }
                    str += mSettleContent;
                    break;
                default:
                    break;
            }
            return str;
        }
    }
}
