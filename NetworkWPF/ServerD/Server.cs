using NetworkWPF.ServerD;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        public static TcpListener listener { get; set; }

        public static bool isRunning = true;

        public static bool isStopped = false;

        public static Stopwatch stopWatch = new Stopwatch();
        
        //服务组件
        public static IdentityService mIdentityService = new IdentityService();
        public static IMService iMService = new IMService();
        public static RoomService mRoomService = new RoomService();


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
            stopWatch.Start();
        }

        public static long GetMilTime()
        {
            return stopWatch.ElapsedMilliseconds;
        }


        //启动
        public async static void Start(TextBlock textBlockIM, TextBlock textBlock,Label labRoomCount,Label labUserCount)
        {
            //绑定UI控件
            messageTextBlock = textBlockIM;
            logTextBlock = textBlock;
            mLabRoomCount = labRoomCount;
            mLabUserCount = labUserCount;

            //开启监听
            listener = new TcpListener(Server.localIP, Server.port);
            listener.Start();
            isRunning = true;
            Log("服务器已启动");
            while (isRunning)
            {
                if (!isStopped)
                {
                    try
                    {
                        //等待连接
                        TcpClient client = await listener.AcceptTcpClientAsync();
                        User user = new User();
                        user.ConnecToClient(client);

                        //注册组件
                        user.Register(mIdentityService);
                        user.Register(iMService);
                        user.Register(mRoomService);


                        //连接成功
                        UpdateStatus();
                        Server.users.Add(user);
                        Log("一个客户端连接了服务器");

                        //返回消息
                        bool b = true;
                        user.Send(new Package(Package.OPT, "LoginPage", "onConnected",b.ToString()));
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

        }









        ///////////////////////////////////////////////////////////////
        ///服务端页面控件刷新
        public static TextBlock logTextBlock;
        public static TextBlock messageTextBlock;
        public static Label mLabRoomCount;
        public static Label mLabUserCount;

        public static void Log(string msg)
        {
            logTextBlock.Dispatcher.InvokeAsync(() =>
            {
                logTextBlock.Text += string.Format("{0}:{1}\n", DateTime.Now.ToString("hh:mm:ss"), msg);
            });
        }

        public static void Message(string msg)
        {
            messageTextBlock.Dispatcher.InvokeAsync(() =>
            {
                messageTextBlock.Text += string.Format("{0}:{1}\n", DateTime.Now.ToString("hh:mm:ss"), msg);
            });
        }

        public static void UpdateStatus()
        {
            mLabUserCount.Dispatcher.InvokeAsync(() =>
            {
                mLabUserCount.Content = users.Count;
            });
            mLabRoomCount.Dispatcher.InvokeAsync(() =>
            {
                mLabRoomCount.Content = RoomService.rooms.Count;
            });
        }
    }
}
