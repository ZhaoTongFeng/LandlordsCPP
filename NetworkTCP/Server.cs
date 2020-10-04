using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTCP
{
    class Server
    {
        public static List<User> users { get; set; }
        public static IPAddress localIP { get; set; }
        public static int port { get; set; }

        private static bool isRunning = true;
        private static bool isStopped = false;

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
        public static void ReStart()
        {
            isRunning = true;
            isStopped = false;
            Start();
        }

        //启动
        public async static void Start()
        {
            isRunning = true;
            //开启监听
            TcpListener listener = new TcpListener(Server.localIP, Server.port);
            listener.Start();
            
            string msg = string.Format("IP:{0},Port:{1}监听中", Server.localIP, Server.port);
            Console.WriteLine(msg);

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
        }
        public static void Stop()
        {
            isStopped = true;
        }
        public static void Continue()
        {
            isStopped = false;
        }

        public static void Shutdown()
        {
            isRunning = false;
        }



        //断开连接之后的处理，移除用户
        public static void RemoveUser(User user)
        {
            users.Remove(user);
        }

        public static void SendToAllClient(string meg)
        {
            for(int i = 0; i < users.Count; i++)
            {
                try
                {
                    users[i].bw.Write(meg);
                    users[i].bw.Flush();
                }
                catch 
                {

                    RemoveUser(users[i]);
                }
            }
        }

        /// <summary>
        /// User可以换成Object
        /// </summary>
        /// <param name="br"></param>
        /// <param name="user"></param>
        public static void ProcessReceive(ref BinaryReader br,User user)
        {
            while (true)
            {
                string receiveString = null;
                try
                {
                    receiveString = br.ReadString();
                }
                catch
                {
                    Server.RemoveUser(user);
                    return;
                }
                string[] split = receiveString.Split(',');
                switch (split[0])
                {
                    case "login":
                        user.name = split[1];
                        Server.SendToAllClient(user.name + "进入了房间," + "在线人数" + Server.users.Count);
                        break;
                    case "logout":
                        Server.SendToAllClient(user.name + "退出了房间。");
                        Server.RemoveUser(user);
                        break;
                    case "msg":
                        Server.SendToAllClient(user.name + "：" + receiveString.Remove(0, 3));
                        break;
                    default:
                        Server.SendToAllClient("报头不匹配，无法解析");
                        break;
                }
            }
        }
    }
}
