using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkTCP
{
    class Program
    {
        public static volatile string ScreenString = "";
        static void Main(string[] args)
        {
            Server.Start();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long mil = 0;
            while (Server.isRunning)
            {
                while (sw.ElapsedMilliseconds - mil < 1000) ;
                mil = sw.ElapsedMilliseconds;
                Console.Clear();
                Console.WriteLine(Server.GetStateString());
                Console.WriteLine("************** Log **************");
                Console.WriteLine(Program.ScreenString);
            }

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
