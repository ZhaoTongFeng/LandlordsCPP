using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NetworkWPF
{

    
    public class Server
    {



        //所有用户列表
        public static List<User> users { get; set; }


        public static IPAddress localIP { get; set; }
        public static int port { get; set; }

        public static bool isRunning = true;
        private static bool isStopped = false;

        //返回服务器状态
        public static string GetStateString()
        {
            string str = "";
            str += String.Format("IP:{0}\n", localIP);
            str += String.Format("Port:{0}\n", port);

            str += "**********************************\n";
            str += String.Format("在线人数:{0}\n", users.Count);
            str += "************ 玩家列表 ************\n";
            string str_user = "{0}\t{1}\t{2}\t\n";
            str += String.Format(str_user, "ID", "Name", "State");
            for (int i = 0; i < users.Count; i++)
            {
                str += String.Format(str_user, i, users[i].name, users[i].GetStateName());
            }

            str += "**********************************\n";
            str += String.Format("房间数:{0}\n", Room.rooms.Count);
            str += "************ 房间列表 ************\n";
            string str_room = "{0}\t{1}\t{2}\t{3}\n";
            str += String.Format(str_room, "ID", "Name", "State", "Num");
            for(int i = 0; i < Room.rooms.Count; i++)
            {
                str += String.Format(str_room, Room.rooms[i].id, Room.rooms[i].name, Room.rooms[i].GetStateName(), Room.rooms[i].users.Count+"/3");
            }
            return str;
        }
        
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


        public static Label mLabRoomCount;
        public static Label mLabUserCount;
        //启动
        public async static void Start(TextBlock textBlock,Label labRoomCount,Label labUserCount)
        {
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
                        User user = new User(client);
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

        public static void SendToAllClient(string msg,User user = null)
        {
            for(int i = 0; i < users.Count; i++)
            {
                if (!users[i].Send(msg))
                {
                    //DisConnected(users[i]);
                }
            }
            if (user != null)
            {
                Log(user.name + "：" + msg);
            }
            else
            {
                Log("系统：" + msg);
            }
            
        }




    }
}
