using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.IO;
using System.Configuration;

namespace NetworkWPF
{

    

    /// <summary>
    /// ClientLoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClientLoginPage : Page,INetwork
    {
        public User user;

        public void SetUser(User user)
        {
            textUserName.Text = user.name;
            textPassword.Text = user.password;
        }

        public ClientLoginPage()
        {
            InitializeComponent();
        }

        public void NextPage()
        {
            //要将User传入下一个Page

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (user.IsConnected())
            {
                user.password = textPassword.Text;
                user.name = textUserName.Text;
                Package package = new Package(Package.OPT, "Identity", "login", JsonSerializer.Serialize(user));
                user.Send(package);
            }
            else
            {
                labelErr.Content = user.GetNetStateName();
            }

        }


        public void onConnected(string data,User sender)
        {
            bool isConnected = Boolean.Parse(data);

            labelErr.Dispatcher.InvokeAsync(() =>
            {
                if (isConnected)
                {
                    labelErr.Content = "网络连接成功";
                    user.mNetState = NetState.Connected;
                }
                else
                {
                    labelErr.Content = "网络连接失败";
                    user.mNetState = NetState.DisConnected;
                }
            });
        }

        public void onLogin(string data, User sender)
        {
            //服务器传回的User，包含从数据库中保存的数据，这里暂时没有对当前的user进行设置
            User user = JsonSerializer.Deserialize<User>(data);
            labelErr.Dispatcher.InvokeAsync(() =>
            {
                sender.isLogin = user.isLogin;
                if (user.isLogin)
                {
                    NextPage();
                    labelErr.Content = "登录成功";
                }
                else
                {
                    textPassword.Text = "";
                    labelErr.Content = "密码错误";
                }
            });

        }

        public void ProcessData(Package package, User sender)
        {
            if (!package.clsName.Equals("LoginPage"))
            {
                return;
            }
            switch (package.funName)
            {
                case "onConnected":
                    onConnected(package.data, sender);
                    break;
                case "onLogin":
                    onLogin(package.data,sender);
                    break;
                default:
                    break;
            }




            ////客户端
            //if (pg.type.Equals(Package.MSG))
            //{
            //    netMsg = pg.data;
            //}
            //else if (pg.type.Equals(Package.OPT))
            //{
            //    //传送给类
            //    //这里是手动传送给模块
            //    //其实可以在类被初始化的时候在Game进行注册，在这儿进行遍历
            //    if (pg.clsName.Equals("Game"))
            //    {
            //        ProcessNetworkPackage(pg);
            //    }
            //    else if (pg.clsName.Equals("Room"))
            //    {
            //        Room.ProcessNetworkPackage(pg);
            //    }
            //    else if (pg.clsName.Equals("GameMode"))
            //    {

            //    }
            //    else
            //    {

            //    }
            //}
            //else
            //{

            //}
        }
    }
}
