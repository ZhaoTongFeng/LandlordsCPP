﻿using NetworkWPF.Client;
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

        public ClientLoginPage clientLoginPage;
        public ClientIMPage clientIMPage;
        public ClientRoomListPage clientroomListPage;
        public ClientRoomPage clientRoomPage;
        public ClientGamePage clientGamePage;
        public ClientGameCallPage clientGameCallPage;
        public ClientGamePreparePage clientGamePreparePage;
        public ClientGameResultPage clientGameResultPage;

        public ClientWindow()
        {
            InitializeComponent();
            //连接服务器
            user = new User();
            user.Register(this);
            Task.Run(() => user.ConnectToServer());
            user.mNetState = NetState.Connecting;

            //初始化Page，注入依赖
            clientLoginPage = new ClientLoginPage();
            clientLoginPage.user = user;
            clientLoginPage.clientWindow = this;
            user.Register(clientLoginPage);

            clientIMPage = new ClientIMPage();
            clientIMPage.user = user;
            clientIMPage.clientWindow = this;
            user.Register(clientIMPage);

            clientroomListPage = new ClientRoomListPage();
            clientroomListPage.user = user;
            clientroomListPage.clientWindow = this;
            user.Register(clientroomListPage);

            clientRoomPage = new ClientRoomPage();
            clientRoomPage.user = user;
            clientRoomPage.clientWindow = this;
            user.Register(clientRoomPage);

            clientGamePage = new ClientGamePage();
            clientGamePage.user = user;
            clientGamePage.clientWindow = this;
            user.Register(clientGamePage);

            clientGameCallPage = new ClientGameCallPage();
            clientGameCallPage.user = user;
            clientGameCallPage.clientWindow = this;
            user.Register(clientGameCallPage);


            clientGamePreparePage = new ClientGamePreparePage();
            clientGamePreparePage.user = user;
            clientGamePreparePage.clientWindow = this;
            user.Register(clientGamePreparePage);

            clientGameResultPage = new ClientGameResultPage();
            clientGameResultPage.user = user;
            clientGameResultPage.clientWindow = this;
            user.Register(clientGameResultPage);


            //设置当前Page
            //mFrameLeft.Source = new Uri("/Client/ClientGamePage.xaml", UriKind.Relative);
            //mFrameLeft.Source = new Uri("/Client/ClientLoginPage.xaml", UriKind.Relative);

            mFrameLeft.Content = clientLoginPage;
            

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
