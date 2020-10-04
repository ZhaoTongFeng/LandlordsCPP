using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTCP
{
    class Program
    {
        static void Main(string[] args)
        {

            Program.Init();
            Console.ReadKey();
        }

        public async static void Init()
        {
            TcpListener listener = new TcpListener(Server.localIP, Server.port);
            listener.Start();
            string msg = string.Format("IP:{0},Port:{1}监听中", Server.localIP, Server.port);
            Console.WriteLine(msg);
            while (true)
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
}
