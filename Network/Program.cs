using System;
using System.Diagnostics;

namespace Network
{
    class Program
    {
        static void Main(string[] args)
        {
            Process mProcess = new Process();
            mProcess.StartInfo.FileName = "Notepad.exe";
            mProcess.StartInfo.Arguments = "D:\\VSProject\\Network\\file\\processfile.txt";
            mProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            mProcess.Start();
            
        }
    }
}
