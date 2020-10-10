using LandlordsCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkWPF.Client
{
    /// <summary>
    /// ClientGameCallPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClientGameCallPage : Page,INetwork
    {
        public User user;

        public ClientWindow clientWindow;

        //没点数
        CardsWidget[] mLeftCardsWidget;
        CardsWidget[] mRightCardsWidget;
        CardsWidget[] mTopCardsWidget;
        CardsWidget[] mCenterCardsWidget;

        //有点数的
        CardsWidget[] mCardsWidgets;

        //扑克牌偏移值
        int pad = 20;

        public ClientGameCallPage()
        {
            InitializeComponent();
            //五个Canvas
            //topCanvas;
            //centerCanvas;
            //leftCanvas;
            //rightCanvas;
            //bottomCanvas;
            App.Current.Dispatcher.InvokeAsync(() => {
                //叫牌

                buttonCallNull.Visibility = Visibility.Hidden;
                buttonCall.Visibility = Visibility.Hidden;
                buttonCallRob.Visibility = Visibility.Hidden;

                //打牌
                buttonOutNull.Visibility = Visibility.Collapsed;
                buttonOut.Visibility = Visibility.Collapsed;

                //手牌数量leftNumLabel

                //队伍名称leftTeamNameLabel

                //出牌倒计时leftTimerLabel

                mCardsWidgets = new CardsWidget[20];
                for (int i = 0; i < mCardsWidgets.Length; i++)
                {
                    mCardsWidgets[i] = new CardsWidget();
                    CardsWidget wi = mCardsWidgets[i];
                    Canvas.SetBottom(wi, 0);
                    Canvas.SetLeft(wi, pad * i);
                    bottomCanvas.Children.Add(wi);
                    wi.Hide();
                }

                mTopCardsWidget = new CardsWidget[3];
                for (int i = 0; i < mTopCardsWidget.Length; i++)
                {
                    mTopCardsWidget[i] = new CardsWidget();
                    CardsWidget wi = mTopCardsWidget[i];
                    Canvas.SetTop(wi, 0);
                    Canvas.SetLeft(wi, pad * i);
                    topCanvas.Children.Add(wi);
                    wi.Hide();
                }

                mLeftCardsWidget = new CardsWidget[20];
                for (int i = 0; i < mLeftCardsWidget.Length; i++)
                {
                    mLeftCardsWidget[i] = new CardsWidget();
                    CardsWidget wi = mLeftCardsWidget[i];
                    Canvas.SetTop(wi, pad * i);
                    Canvas.SetLeft(wi, 0);
                    leftCanvas.Children.Add(wi);
                    wi.Hide();

                }
                mRightCardsWidget = new CardsWidget[20];
                for (int i = 0; i < mRightCardsWidget.Length; i++)
                {
                    mRightCardsWidget[i] = new CardsWidget();
                    CardsWidget wi = mRightCardsWidget[i];
                    Canvas.SetTop(wi, pad * i);
                    Canvas.SetLeft(wi, 0);
                    rightCanvas.Children.Add(wi);
                    wi.Hide();
                }


                mCenterCardsWidget = new CardsWidget[20];
                for (int i = 0; i < mCenterCardsWidget.Length; i++)
                {
                    mCenterCardsWidget[i] = new CardsWidget();
                    CardsWidget wi = mCenterCardsWidget[i];
                    Canvas.SetTop(wi, 0);
                    Canvas.SetLeft(wi, pad * i);
                    centerCanvas.Children.Add(wi);
                    wi.Hide();
                }
            });

        }


        public void ShowCall(int n)
        {
            if (n == 0)
            {
                //不叫
                buttonCallNull.Visibility = Visibility.Visible;
            }else if (n == 1)
            {
                //不叫，叫地主
                buttonCallNull.Visibility = Visibility.Visible;
                buttonCall.Visibility = Visibility.Visible;
            }else if (n == 2)
            {
                //不叫，抢地主
                buttonCallNull.Visibility = Visibility.Visible;
                buttonCallRob.Visibility = Visibility.Visible;
            }
        }
        private void HideCall()
        {
            buttonCall.Visibility = Visibility.Hidden;
            buttonCallNull.Visibility = Visibility.Hidden;
            buttonCallRob.Visibility = Visibility.Hidden;
        }
        public void ShowOut(int n)
        {
            if (n == 0)
            {
                //不要
                buttonOutNull.Visibility = Visibility.Visible;
            }else if (n == 1)
            {
                //出牌
                buttonOut.Visibility = Visibility.Visible;
            }else if (n == 2)
            {
                //不要，出牌
                buttonOutNull.Visibility = Visibility.Visible;
                buttonOut.Visibility = Visibility.Visible;
            }
        }
        private void HideOut()
        {
            buttonOutNull.Visibility = Visibility.Hidden;
            buttonOut.Visibility = Visibility.Hidden;
        }



        private void CALL_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string data = "";
            if (button != null)
            {

                switch (button.Name)
                {
                    case "buttonCallNull":
                        data = "不叫";
                        break;
                    case "buttonCall":
                        data = "叫地主";
                        break;
                    case "buttonCallRob":
                        data = "抢地主";
                        break;
                    default:
                        break;
                }
                user.Send(new Package(Package.OPT, "LandlordsGameMode", "Call", data.ToString()));
            }
        }



        private void OUT_CLICK(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            List<int> indexs = new List<int>();
            if (button != null)
            {
                for(int i = 0; i < mCardsWidgets.Length; i++)
                {
                    if (mCardsWidgets[i].isUp)
                    {
                        indexs.Add(i);
                    }
                }
                user.Send(new Package(Package.OPT, "LandlordsGameMode", "HandOut", JsonSerializer.Serialize(indexs)));
            }
        }


        public void ProcessData(Package package, User sender)
        {
            if (!package.clsName.Equals("GameCallPage"))
            {
                return;
            }
            switch (package.funName)
            {
                case "onStart":
                    onStart(package.data, sender);
                    break;
                case "onTimer":
                    onTimer(package.data, sender);
                    break;
                case "onCall":
                    onCall(package.data, sender);
                    break;
                case "onCallFinish":
                    onCallFinish(package.data, sender);
                    break;
                case "onHandOut":
                    onHandOut(package.data, sender);
                    break;
                case "onGameFinish":
                    onGameFinish(package.data, sender);
                    break;
                default:
                    break;
            }
        }

        public void onStart(string data, User sender)
        {
            //初始化游戏之后，这里接受手牌，和数量
            Dictionary<string, string> dic = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
            
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                //手牌
                CardsBuf cards = JsonSerializer.Deserialize<CardsBuf>(dic["cards"]);

                List<int> buf = cards.buf;

                for (int i = 0; i < 17; i++)
                {
                    mCardsWidgets[i].Show();
                    mCardsWidgets[i].SetNum(CardsBuf.GetCardName(buf[i]));
                }

                //手牌数量
                int[] count_cards = JsonSerializer.Deserialize<int[]>(dic["count_cards"]);
                //右边，顺时针方向
                for (int i = 0; i < 17; i++)
                {
                    mRightCardsWidget[i].Show();
                
                }
                //左边
                for (int i = 0; i < 17; i++)
                {
                    mLeftCardsWidget[i].Show();
                }
                //手牌数量
                bottomNumLabel.Content = buf.Count.ToString();
                rightNumLabel.Content = count_cards[0].ToString();
                leftNumLabel.Content = count_cards[1].ToString();


                //当前游戏阶段
                GameSession gameSession = (GameSession)System.Enum.Parse(typeof(GameSession), dic["GameSession"]);

                
                if (dic.ContainsKey("this"))
                {
                    //首个叫地主的玩家，本玩家为当前操作玩家
                    int n = int.Parse(dic["this"]);
                    if (gameSession == GameSession.CALL)
                    {
                        ShowCall(n);
                    }else if (gameSession == GameSession.PLAYING)
                    {
                        ShowOut(n);
                    }
                }

                mIndexInRoom = int.Parse(dic["indexInRoom"]);
                mCurOptIndex = int.Parse(dic["curOptIndex"]);

            });

        }

        /// <summary>
        /// 在房间玩家列表中的位置
        /// </summary>
        private int mIndexInRoom;

        private int mCurOptIndex;

        public void onTimer(string data, User sender)
        {
            //这里对计时器进行更新
            //三个玩家是同步的

            //如果计时器结束后，此玩家是当前操作玩家，则会自动调用各个阶段的默认事件
            int tick = int.Parse(data);
            App.Current.Dispatcher.InvokeAsync(() => {
                leftTimerLabel.Content = "";
                rightTimerLabel.Content = "";
                bottomTimerLabel.Content = "";

                //bottomTimerLabel.Visibility = Visibility.Hidden;
                //leftTimerLabel.Visibility = Visibility.Hidden;
                //rightTimerLabel.Visibility = Visibility.Hidden;
                if (mIndexInRoom == mCurOptIndex)
                {
                    bottomTimerLabel.Content = tick.ToString();
                    bottomTimerLabel.Visibility = Visibility.Visible;
                }
                else if ((mIndexInRoom + 2) % 3 == mCurOptIndex)
                {
                    leftTimerLabel.Content = tick.ToString();
                    leftTimerLabel.Visibility = Visibility.Visible;
                }
                else if ((mIndexInRoom + 1) % 3 == mCurOptIndex)
                {
                    rightTimerLabel.Content = tick.ToString();
                    rightTimerLabel.Visibility = Visibility.Visible;
                }
            });
        }


        public void onCall(string data, User sender)
        {
            Dictionary<string, string> dic = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
            App.Current.Dispatcher.InvokeAsync(() => {

                string callStateName = GetCallStateName(dic["callState"]);
                mCurOptIndex = int.Parse(dic["curOptIndex"]);
                
                HideCall();
                if(mCurOptIndex== mIndexInRoom)
                {
                    int showState = int.Parse(dic["showState"]);
                    ShowCall(showState);
                    bottomTimerLabel.Content = callStateName;
                }
                else if ((mIndexInRoom + 2) % 3 == mCurOptIndex)
                {
                    leftTimerLabel.Content = callStateName;
                }
                else if ((mIndexInRoom + 1) % 3 == mCurOptIndex)
                {
                    rightTimerLabel.Content = callStateName;
                }
            });
        }

        public void onCallFinish(string data, User sender)
        {
            Dictionary<string, string> dic = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
            App.Current.Dispatcher.InvokeAsync(() => {

                int LandlordsIndex = int.Parse(dic["LandlordsIndex"]);
                CardsBuf cards = JsonSerializer.Deserialize<CardsBuf>(dic["DarkCards"]);

                mCurOptIndex = LandlordsIndex;


                HideCall();

                //显示底牌
                for (int i = 0; i < 3; i++)
                {
                    mTopCardsWidget[i].SetNum(CardsBuf.GetCardName(cards.buf[i]));
                    mTopCardsWidget[i].Show();
                }

                if (mIndexInRoom == mCurOptIndex)
                {
                    //设置队伍状态
                    bottomTeamNameLabel.Content = "地主";
                    leftTeamNameLabel.Content = "农民";
                    rightTeamNameLabel.Content = "农民";

                    //更新手牌数量
                    bottomNumLabel.Content = "20";

                    //添加三张手牌

                    for(int i = 17; i < 20; i++)
                    {

                        mCardsWidgets[i].SetNum(CardsBuf.GetCardName(cards.buf[i - 17]));
                        mCardsWidgets[i].Visibility = Visibility.Visible;
                    }

                    //此回合
                    int showState = int.Parse(dic["showState"]);
                    ShowOut(showState);
                }
                else if ((mIndexInRoom + 2) % 3 == mCurOptIndex)
                {
                    leftNumLabel.Content = "20";

                    bottomTeamNameLabel.Content = "农民";
                    leftTeamNameLabel.Content = "地主";
                    rightTeamNameLabel.Content = "农民";
                }
                else if ((mIndexInRoom + 1) % 3 == mCurOptIndex)
                {
                    rightNumLabel.Content = "20";

                    bottomTeamNameLabel.Content = "农民";
                    leftTeamNameLabel.Content = "农民";
                    rightTeamNameLabel.Content = "地主";
                }



            });
        }

        public void onHandOut(string data, User sender)
        {
            
            Dictionary<string, string> dic = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
            App.Current.Dispatcher.InvokeAsync(() => {
                
                int isSucc = int.Parse(dic["isSucc"]);
                if (isSucc != 0)
                {
                    return;
                }
                HideOut();
                //更新手牌数量
                int[] count_cards = JsonSerializer.Deserialize<int[]>(dic["count_cards"]);
                bottomNumLabel.Content = count_cards[mIndexInRoom].ToString();
                rightNumLabel.Content = count_cards[(mIndexInRoom+1)%3].ToString();
                leftNumLabel.Content = count_cards[(mIndexInRoom+2)%3].ToString();

                //把打出的牌添加到中间
                for (int i = 0; i < mCenterCardsWidget.Length; i++)
                {
                    mCenterCardsWidget[i].Hide();
                }
                CardsBuf LastCards = JsonSerializer.Deserialize<CardsBuf>(dic["LastCards"]);
                if (LastCards.GetSize() != 0)
                {
                    for (int i = 0; i < LastCards.GetSize(); i++)
                    {
                        CardsWidget wi = mCenterCardsWidget[i];
                        wi.Show();
                        wi.SetNum(CardsBuf.GetCardName(LastCards.buf[i]));
                    }
                }

                //当前玩家的上一个玩家手牌减掉
                //其实就等于把当前剩余的牌发给他，然后重新显示
                if (dic.ContainsKey("CurrentCards"))
                {
                    CardsBuf CurrentCards = JsonSerializer.Deserialize<CardsBuf>(dic["CurrentCards"]);
                    for (int i = 0; i < mCardsWidgets.Length; i++)
                    {
                        mCardsWidgets[i].Hide();
                    }
                    for (int i = 0; i < CurrentCards.GetSize(); i++)
                    {
                        CardsWidget wi = mCenterCardsWidget[i];
                        wi.Show();
                        wi.SetNum(CardsBuf.GetCardName(CurrentCards.buf[i]));
                    }
                }


                //当前玩家显示按钮
                mCurOptIndex = int.Parse(dic["curOptIndex"]);
                if (mIndexInRoom == mCurOptIndex)
                {
                    int showState = int.Parse(dic["showState"]);
                    ShowOut(showState);
                }
            });
        }

        public void onGameFinish(string data, User sender)
        {
            //游戏结束之后
            //弹出结算窗口
            //结算窗口除基本信息以外只显示确定按钮
        }

        private string GetCallStateName(string state)
        {
            if (state.Equals("no"))
            {
                return "不叫";
            }
            else if (state.Equals("call"))
            {
                return "叫地主";
            }
            else if (state.Equals("rob"))
            {
                return "抢地主";
            }
            else
            {
                return "";
            }
        }
    }



}
