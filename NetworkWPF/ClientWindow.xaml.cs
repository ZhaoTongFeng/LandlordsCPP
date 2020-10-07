using NetworkWPF.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetworkWPF
{
    /// <summary>
    /// ClientWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClientWindow : Window, INetwork
    {
        ////////////////////////////////////////////////////////////
        /// <summary>
        /// 测试用
        /// </summary>
        public int index;
        public static List<User> users = new List<User>()
        {
            { new User(0,"USER-1","123456") },
            { new User(1,"USER-2","123456") },
            { new User(2,"USER-3","123456") },
            { new User(3,"USER-4","123456") },
        };
        public void SetUser(int index)
        {
            this.index = index;
            //ClientLoginPage page = mFrame.Content as ClientLoginPage;
            clientLoginPage.SetUser(users[index]);
        }
        ////////////////////////////////////////////////////////////






        public User user;


        ClientLoginPage clientLoginPage;
        ClientIMPage clientIMPage;



        public ClientWindow()
        {
            InitializeComponent();
            //连接服务器
            user = new User();
            user.Register(this);
            Task.Run(() => user.ConnectToServer());
            user.mNetState = NetState.Connecting;
            //mFrameLeft.Source = new Uri("/Client/ClientGamePage.xaml", UriKind.Relative);
            //mFrameLeft.Source = new Uri("/Client/ClientLoginPage.xaml", UriKind.Relative);

            clientLoginPage = new ClientLoginPage();
            clientLoginPage.user = user;
            user.Register(clientLoginPage);
            clientIMPage = new ClientIMPage();
            clientIMPage.user = user;
            user.Register(clientIMPage);

            mFrameLeft.Content = clientLoginPage;
            mFrameRight.Content = clientIMPage;

            //clientIMPage.Visibility = Visibility.Collapsed;

        }




        public void ProcessData(Package package, User sender)
        {
            if (!package.clsName.Equals("ClientWindow"))
            {
                return;
            }
            switch (package.funName)
            {
                default:
                    break;
            }
        }
    }
}
