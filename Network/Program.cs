using System;
using System.Diagnostics;
using System.Threading;

namespace Network
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string server = "F:\\VSProject\\ZhaoTongFeng\\LandlordsCPP\\NetworkTCP\\obj\\Debug\\NetworkTCP.exe";
            string client = "F:\\VSProject\\ZhaoTongFeng\\LandlordsCPP\\NetworkTCPClient\\obj\\Debug\\NetworkTCPClient.exe";
            OpenExe(server);

            OpenExe(client);
        }

        public static void OpenExe(string name)
        {
            Process mProcess = new Process();
            mProcess.StartInfo.FileName = name;
            //mProcess.StartInfo.Arguments = "D:\\VSProject\\Network\\file\\processfile.txt";
            mProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            mProcess.Start();
        }
    }
}
