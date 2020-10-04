using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Start();
            Console.ReadKey();

            //Thread thread = new Thread(CommandControl);
            //thread.Start();
        }

        public static void CommandControl()
        {
            while (true)
            {
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "start":
                        Server.Start();
                        break;
                    case "stop":
                        Server.Stop();
                        break;
                    case "shutdown":
                        Server.Shutdown();
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
