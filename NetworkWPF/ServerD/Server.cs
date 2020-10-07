using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;

namespace NetworkWPF
{
    public class Server
    {
        //所有用户列表
        public static List<User> users { get; set; }
        public static IPAddress localIP { get; set; }
        public static int port { get; set; }

        private static bool isRunning = true;
        private static bool isStopped = false;

        public static IdentityService mIdentityService = new IdentityService();
        public static IMService iMService = new IMService();
        static Server()
        {
            users = new List<User>();
            port = 51888;
            //找一个ipv4的地址
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var v in ips)
            {
                if (v.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = v;
                    break;
                }
            }
        }

        public static TcpListener listener;



        public static TextBlock logTextBlock;
        public static void Log(string msg)
        {
            logTextBlock.Dispatcher.InvokeAsync(() =>
            {
                logTextBlock.Text += string.Format("{0}:{1}\n", DateTime.Now.ToString("hh:mm:ss"), msg);
            });
        }

        public static TextBlock messageTextBlock;
        public static void Message(string msg)
        {
            messageTextBlock.Dispatcher.InvokeAsync(() =>
            {
                messageTextBlock.Text += string.Format("{0}:{1}\n", DateTime.Now.ToString("hh:mm:ss"), msg);
            });
        }

        public static Label mLabRoomCount;
        public static Label mLabUserCount;


        public static void UpdateStatus()
        {
            mLabUserCount.Dispatcher.InvokeAsync(() =>
            {
                mLabUserCount.Content = users.Count;
            });
            mLabRoomCount.Dispatcher.InvokeAsync(() =>
            {
                mLabRoomCount.Content = Room.rooms.Count;
            });
        }

        //启动
        public async static void Start(TextBlock textBlockIM, TextBlock textBlock,Label labRoomCount,Label labUserCount)
        {
            messageTextBlock = textBlockIM;
            logTextBlock = textBlock;
            mLabRoomCount = labRoomCount;
            mLabUserCount = labUserCount;

            //开启监听
            listener = new TcpListener(Server.localIP, Server.port);
            listener.Start();

            Log("服务器已启动");

            isRunning = true;
            while (isRunning)
            {
                if (!isStopped)
                {
                    try
                    {
                        TcpClient client = await listener.AcceptTcpClientAsync();
                        User user = new User();
                        user.ConnecToClient(client);
                        user.Register(mIdentityService);
                        user.Register(iMService);
                        Log("一个客户端连接了服务器");
                        UpdateStatus();
                        bool b = true;
                        user.Send(new Package(Package.OPT, "LoginPage", "onConnected",b.ToString()));
                        Server.users.Add(user);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            Log("服务器已关闭");
        }


        public static void Stop()
        {
            isStopped = true;
            if (listener != null)
            {
                listener.Stop();
            }
            Log("服务器已关闭");
        }

        public static void SendToAll(Package pg)
        {
            for(int i = 0; i < users.Count; i++)
            {
                if (!users[i].Send(pg))
                {
                    users[i].DisConnected();
                }
            }
        }

        //不能在静态函数中使用this
        public void ProcessData(Package pg, User sender)
        {
            //if (package.type == null)
            //{

            //}
            //else if (package.type.Equals(Package.MSG))
            //{
            //    MessageSender.SendToAllUsers(this, package.data);
            //}
            //else if (package.type.Equals(Package.OPT))
            //{
            //    if (package.clsName.Equals("Room"))
            //    {
            //        switch (package.funName)
            //        {
            //            case "create": onCreateRoom(); break;
            //            case "join": onJoinRoom(root); break;
            //            case "start": onStartGame(); break;
            //            case "exit": onExitRoom(); break;
            //            default: Send("msg," + "报头不匹配，无法解析"); break;
            //        }
            //    }
            //    else if (package.clsName.Equals("User"))
            //    {
            //        switch (package.funName)
            //        {
            //            case "login": onLogin(root); break;
            //            case "logout": onLogout(); break;
            //            default: break;
            //        }
            //    }
            //}
            //else
            //{

            //}
        }
    }
}
