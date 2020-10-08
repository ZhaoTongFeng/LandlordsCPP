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
            user.Send(new Package(Package.OPT, "Room", "GetList", ""));
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            user.Send(new Package(Package.OPT, "Room", "Create", ""));
        }

        private void ButtonJoin_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int id = int.Parse(button.Tag.ToString());
            user.Send(new Package(Package.OPT, "Room", "Join", id.ToString()));
        }


        public void ProcessData(Package package, User sender)
        {
            if (!package.clsName.Equals("RoomListPage"))
            {
                return;
            }
            switch (package.funName)
            {
                case "onCreatedRoom":
                    onCreatedRoom(package.data, sender);
                    break;
                case "onJoinRoom":
                    onJoinRoom(package.data, sender);
                    break;


                case "onGetList":
                    onGetList(package.data, sender);
                    break;
                case "onInsert":
                    onInsert(package.data, sender);
                    break;
                case "onUpdate":
                    onUpdate(package.data, sender);
                    break;
                case "onDelete":
                    onDelete(package.data, sender);
                    break;

                default:
                    break;
            }
        }





        public void JumpToGamePage()
        {
            clientWindow.clientRoomPage.Show(this);
        }

        //下面两个进行跳转操作
        public void onCreatedRoom(string data, User sender)
        {
            Room room = JsonSerializer.Deserialize<Room>(data);
            //跳转房间
            JumpToGamePage();
        }

        public void onJoinRoom(string data, User sender)
        {
            Room room = JsonSerializer.Deserialize<Room>(data);
            //跳转房间
            JumpToGamePage();
        
        }

        //控件增删改
        Dictionary<int, RoomWidget> roomWidgets = new Dictionary<int, RoomWidget>();
        public void AddRoomWidget(Room room)
        {
            App.Current.Dispatcher.InvokeAsync(() => {
                RoomWidget roomWidget = new RoomWidget();

                roomWidget.labelRoomName.Content = room.name;

                roomWidget.buttonJoin.Tag = room.id;
                roomWidget.buttonJoin.Click += ButtonJoin_Click;

                wrapPanelRoom.Children.Add(roomWidget);
                roomWidgets.Add(room.id, roomWidget);
            });
        }
        public void RemoveRoomWidget(int id)
        {
            App.Current.Dispatcher.InvokeAsync(() => {
                RoomWidget roomWidget = roomWidgets[id];
                wrapPanelRoom.Children.Remove(roomWidget);
                roomWidgets.Remove(id);
            });
        }
        public void UpdateRoomWidget(Room room)
        {
            App.Current.Dispatcher.InvokeAsync(() => {
                RoomWidget roomWidget = roomWidgets[room.id];
                roomWidget.onUpdate(room);
            });
        }
        //下面四个函数只是数据的变化，而不做跳转处理

        //更新所有控件
        //增删改都放到这一个函数中了，方便倒是方便了，就是任何一个小改动都需要去遍历整个字典
        //所以这个函数只是在加载此页面时调用一次，加载当前完整的列表
        //在每个单独操作的时候，还是只调用各自的操作函数即可。
        public void onGetList(string data, User sender)
        {
            Dictionary<string, Room> rooms = JsonSerializer.Deserialize<Dictionary<string, Room>>(data);
            foreach (var item in rooms)
            {
                int id = int.Parse(item.Key);
                Room room = item.Value;
                if (roomWidgets.ContainsKey(id))
                {
                    //更新
                    UpdateRoomWidget(room);
                }
                else
                {
                    //增加
                    AddRoomWidget(room);
                }
            }
            foreach (var item in roomWidgets)
            {
                int id = item.Key;
                RoomWidget roomWidget = item.Value;

                if (!rooms.ContainsKey(id.ToString()))
                {
                    //删除
                    RemoveRoomWidget(id);
                }
            }
        }

        public void onInsert(string data, User sender)
        {
            //增加
            Room room = JsonSerializer.Deserialize<Room>(data);
            AddRoomWidget(room);
        }
        public void onUpdate(string data, User sender)
        {
            //更新
            Room room = JsonSerializer.Deserialize<Room>(data);
            UpdateRoomWidget(room);
        }
        public void onDelete(string data,User user)
        {
            //删除
            int id = int.Parse(data);
            RemoveRoomWidget(id);
        }
    }
}
