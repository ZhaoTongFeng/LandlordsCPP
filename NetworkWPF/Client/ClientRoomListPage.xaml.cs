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
    /// ClientRoomListPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClientRoomListPage : Page,INetwork
    {
        public User user;
        public ClientWindow clientWindow;
        public ClientRoomListPage()
        {
            InitializeComponent();
            
        }
        public void Show(Page fromPage)
        {
            clientWindow.mFrameLeft.Content = this;
            try
            {
                ClientLoginPage page = fromPage as ClientLoginPage;
                clientWindow.mFrameRight.Content = clientWindow.clientIMPage;

            }
            catch 
            {

            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            user.Send(new Package(Package.OPT, "Room", "Create", ""));
        }

        private void ButtonJoin_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int index = int.Parse(button.Tag.ToString());
            RoomWidget roomWidget = roomWidgets[index];

            string id = roomWidget.labelRoomId.Content.ToString();

            user.Send(new Package(Package.OPT, "Room", "Join", id));
        }


        public void ProcessData(Package package, User sender)
        {
            if (!package.clsName.Equals("RoomListPage"))
            {
                return;
            }
            switch (package.funName)
            {
                case "onGetList":
                    onGetList(package.data, sender);
                    break;
                case "onCreatedRoom":
                    onCreatedRoom(package.data, sender);
                    break;
                case "onJoinRoom":
                    onJoinRoom(package.data, sender);
                    break;

                default:
                    break;
            }
        }


        List<RoomWidget> roomWidgets = new List<RoomWidget>();
        Dictionary<int, Room> rooms = new Dictionary<int, Room>();

        public void AddRoomWidget(Room room)
        {
            App.Current.Dispatcher.InvokeAsync(() => {
                RoomWidget roomWidget = new RoomWidget();
                roomWidget.labelRoomId.Content = room.id;
                roomWidget.labelRoomName.Content = room.name;

                roomWidget.buttonJoin.Tag = roomWidgets.Count;
                roomWidget.buttonJoin.Click += ButtonJoin_Click;

                wrapPanelRoom.Children.Add(roomWidget);
                roomWidgets.Add(roomWidget);
            });
        }

        public void onCreatedRoom(string data, User sender)
        {
            Room room = JsonSerializer.Deserialize<Room>(data);

            AddRoomWidget(room);
            //上面已经房间已经创建成功了。

            //这儿加入房间
        }

        public void onGetList(string data, User sender)
        {
            Room room = JsonSerializer.Deserialize<Room>(data);

        }


        public void onJoinRoom(string data, User sender)
        {
            Room room = JsonSerializer.Deserialize<Room>(data);

        }
    }
}
