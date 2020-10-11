using NetworkWPF.Public;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
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
    /// ClientRoomPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClientRoomPage : Page,INetwork
    {
        public ClientRoomPage()
        {
            InitializeComponent();
        }

        public User user;
        public ClientWindow clientWindow;

        public void Show(Page frompage)
        {
            App.Current.Dispatcher.InvokeAsync(() => {
                clientWindow.mFrameLeft.Content = this;
            });
            SoundPlayer soundPlayer = new SoundPlayer();

            soundPlayer.SoundLocation = @"D:\VSProject\ZhaoTongFeng\LandlordsCPP\NetworkWPF\Music\background.wav";

            soundPlayer.Load();
            //soundPlayer.Play();
            soundPlayer.PlayLooping();
        }

        private void PrepareBtn_Click(object sender, RoutedEventArgs e)
        {

            user.Send(new Package(Package.OPT, "Room", "Prepare", ""));
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            user.Send(new Package(Package.OPT, "Room", "Exit", ""));
        }

        public void ProcessData(Package package, User sender)
        {
            if (!package.clsName.Equals("RoomPage"))
            {
                return;
            }
            switch (package.funName)
            {
                case "onPrepare":
                    onPrepare(package.data, sender);
                    break;
                case "onExit":
                    onExit(package.data, sender);
                    break;
                case "onStartGame":
                    onStartGame(package.data, sender);
                    break;

                case "onUpdate":
                    onUpdate(package.data, sender);
                    break;
                case "onTestDelay":
                    onTestDelay(package.data, sender);
                    break;
                default:
                    break;
            }
        }

        public void onPrepare(string data, User sender)
        {
            App.Current.Dispatcher.InvokeAsync(() => {
                PrepareBtn.Content = "准备就绪";
                PrepareBtn.IsEnabled = false;
            });


        }
        public void onExit(string data, User sender)
        {

        }

        public void onStartGame(string data, User sender)
        {
            App.Current.Dispatcher.InvokeAsync(() => {
                mGameFrame.Content = clientWindow.clientGameCallPage;
            });
        }

        public void onUpdate(string data, User sender)
        {
            //更新房间内的玩家信息
            List<User> users = JsonSerializer.Deserialize<List<User>>(data);

            var q = from u in users
                    select new
                    {
                        名称 = u.name,
                        状态 = u.GetPrepareName()
                    };
            App.Current.Dispatcher.InvokeAsync(() => {
                mDataGrid.ItemsSource = q.ToList();
            });
            
        }

        public void onTestDelay(string data, User sender)
        {
            try
            {
                long last = long.Parse(data);
                long delay = Util.GetTimeStamp() - last;
                App.Current.Dispatcher.InvokeAsync(() => {
                    labelDelay.Content = "延迟" + delay + " ms";
                });
            }
            catch
            {

            }

        }


    }
}
