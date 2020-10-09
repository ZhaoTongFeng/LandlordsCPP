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

            mTopCardsWidget = new CardsWidget[3];
            for (int i = 0; i < mTopCardsWidget.Length; i++)
            {
                mTopCardsWidget[i] = new CardsWidget();
                CardsWidget wi = mTopCardsWidget[i];
                Canvas.SetTop(wi, 0);
                Canvas.SetLeft(wi, pad * i);
                topCanvas.Children.Add(wi);

            }
            mLeftCardsWidget = new CardsWidget[20];
            for (int i = 0; i < mLeftCardsWidget.Length; i++)
            {
                mLeftCardsWidget[i] = new CardsWidget();
                CardsWidget wi = mLeftCardsWidget[i];
                Canvas.SetTop(wi, pad * i);
                Canvas.SetLeft(wi, 0);
                leftCanvas.Children.Add(wi);

            }
            mRightCardsWidget = new CardsWidget[20];
            for (int i = 0; i < mRightCardsWidget.Length; i++)
            {
                mRightCardsWidget[i] = new CardsWidget();
                CardsWidget wi = mRightCardsWidget[i];
                Canvas.SetTop(wi, pad * i);
                Canvas.SetLeft(wi, 0);
                rightCanvas.Children.Add(wi);

            }
            mCardsWidgets = new CardsWidget[20];
            for (int i = 0; i < mCardsWidgets.Length; i++)
            {
                mCardsWidgets[i] = new CardsWidget();
                CardsWidget wi = mCardsWidgets[i];
                Canvas.SetTop(wi, 0);
                Canvas.SetLeft(wi, pad * i);
                bottomCanvas.Children.Add(wi);

            }

            mCenterCardsWidget = new CardsWidget[20];
            for (int i = 0; i < mCenterCardsWidget.Length; i++)
            {
                mCenterCardsWidget[i] = new CardsWidget();
                CardsWidget wi = mCenterCardsWidget[i];
                Canvas.SetTop(wi, 0);
                Canvas.SetLeft(wi, pad * i);
                centerCanvas.Children.Add(wi);
            }
        }


        public void ShowCall(int n)
        {
            if (n == 0)
            {
                //不叫
                buttonCall.Visibility = Visibility.Visible;
            }else if (n == 1)
            {
                //不叫，叫地主
                buttonCall.Visibility = Visibility.Visible;
                buttonCallNull.Visibility = Visibility.Visible;
            }else if (n == 2)
            {
                //不叫，抢地主
                buttonCall.Visibility = Visibility.Visible;
                buttonCallRob.Visibility = Visibility.Visible;
            }
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


        private void CALL_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            CallState call = CallState.NO;
            if (button != null)
            {

                switch (button.Name)
                {
                    case "buttonCallNull":
                        call = CallState.NO;
                        break;
                    case "buttonCall":
                        call = CallState.CALL;
                        break;
                    case "buttonCallRob":
                        call = CallState.Rob;
                        break;
                    default:
                        break;
                }
                user.Send(new Package(Package.OPT, "LandlordsGameMode", "Call", call.ToString()));
            }
        }

        private void OUT_CLICK(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                if (button.Name.Equals("buttonOutNull"))
                {

                }else if (button.Name.Equals("buttonOut"))
                {

                }
                else
                {

                }
                user.Send(new Package(Package.OPT, "LandlordsGameMode", "HandOut", ""));
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

                for (int i = 0; i < mCardsWidgets.Length; i++)
                {
                    if (i < buf.Count)
                    {
                        mCardsWidgets[i].Visibility = Visibility.Visible;
                        mCardsWidgets[i].SetNum(CardsBuf.GetCardName(buf[i]));

                    }
                    else
                    {
                        mCardsWidgets[i].Visibility = Visibility.Hidden;
                    }
                }

                //手牌数量
                int[] count_cards = JsonSerializer.Deserialize<int[]>(dic["count_cards"]);
                //右边，顺时针方向
                for (int i = 0; i < mRightCardsWidget.Length; i++)
                {
                    if (i < count_cards[0])
                    {
                        mRightCardsWidget[i].Visibility = Visibility.Visible;

                    }
                    else
                    {
                        mRightCardsWidget[i].Visibility = Visibility.Hidden;
                    }
                }
                //左边
                for (int i = 0; i < mLeftCardsWidget.Length; i++)
                {
                    if (i < count_cards[1])
                    {
                        mLeftCardsWidget[i].Visibility = Visibility.Visible;

                    }
                    else
                    {
                        mLeftCardsWidget[i].Visibility = Visibility.Hidden;
                    }
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

                indexInRoom = int.Parse(dic["indexInRoom"]);
                curOptIndex = int.Parse(dic["curOptIndex"]);

            });

        }

        /// <summary>
        /// 在房间玩家列表中的位置
        /// </summary>
        private int indexInRoom;

        private int curOptIndex;

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

                if (indexInRoom == curOptIndex)
                {
                    bottomTimerLabel.Content = tick.ToString();
                }
                else if ((indexInRoom + 1) % 3 == curOptIndex)
                {
                    leftTimerLabel.Content = tick.ToString();

                }
                else if ((indexInRoom + 2) % 3 == curOptIndex)
                {
                    rightTimerLabel.Content = tick.ToString();
                }
            });
        }

        public void onCall(string data, User sender)
        {
            //别人叫牌完毕之后，也有三种几种结果，根据服务器传入的指示，显示部分叫牌按钮
            buttonCallNull.Visibility = Visibility.Hidden;
            buttonCall.Visibility = Visibility.Hidden;
            buttonCallRob.Visibility = Visibility.Hidden;
        }

        public void onCallFinish(string data, User sender)
        {
            //叫牌结束之后，将队伍状态和新的手牌数量进行发送并3张底牌进行显示
        }

        public void onOut(string data, User sender)
        {
            //别人或者自己出牌之后，结果只有两个，不要，和出牌，
            //先显示别人的操作
            //再服务器传回应该显示哪些按钮
            buttonOutNull.Visibility = Visibility.Hidden;
            buttonOut.Visibility = Visibility.Hidden;
        }

        public void onGameFinish(string data, User sender)
        {
            //游戏结束之后弹出结算窗口，结算窗口除基本信息以外只显示确定按钮
        }
    }
}
