using System;
using System.Collections.Generic;
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


    }
}
