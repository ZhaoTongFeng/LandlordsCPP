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

    
    public class Server
    {

        //房间列表
        public static Dictionary<int, Room> rooms;
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
            str += String.Format("房间数:{0}\n", rooms.Count);
            str += "************ 房间列表 ************\n";
            string str_room = "{0}\t{1}\t{2}\t{3}\n";
            str += String.Format(str_room, "ID", "Name", "State", "Num");
            for(int i = 0; i < rooms.Count; i++)
            {
                str += String.Format(str_room, rooms[i].id, rooms[i].name, rooms[i].GetStateName(), rooms[i].users.Count+"/3");
            }
            return str;
        }
        
        static Server()
        {
            rooms = new Dictionary<int, Room>();
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

        //启动
        public async static void Start()
        {
            isRunning = true;
            //开启监听
            TcpListener listener = new TcpListener(Server.localIP, Server.port);
            listener.Start();
            
            string msg = string.Format("IP:{0},Port:{1}监听中", Server.localIP, Server.port);
            //Console.WriteLine(msg);
            Program.ScreenString += msg + "\n";

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
            Console.WriteLine("监听结束");
        }


        //断开连接之后的处理，移除用户
        public static void DisConnected(User user)
        {
            SendToAllClient(user.name + "下线了");
            users.Remove(user);
        }


        public static void SendToAllClient(string msg)
        {
            for(int i = 0; i < users.Count; i++)
            {
                if (!users[i].Send(msg))
                {
                    DisConnected(users[i]);
                }
            }
            lock (Program.ScreenString)
            {
                Program.ScreenString += msg + "\n";
            }
            
        }


        public static void ReStart()
        {
            isRunning = true;
            isStopped = false;
            Start();
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
    }
}
