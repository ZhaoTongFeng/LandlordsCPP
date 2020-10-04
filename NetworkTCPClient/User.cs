using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkTCPClient
{
    class User
    {
        public string name { get; set; }
        public TcpClient client { get; set; }
        public BinaryReader br { get; set; }
        public BinaryWriter bw { get; set; }
        public string remoteHost { get; set; }

        public int remotePort = 51888;

        public bool isConnected = true;
        public User()
        {
            remoteHost = Dns.GetHostName();
        }
        /// <summary>
        /// 输出到屏幕
        /// </summary>
        /// <param name="content"></param>
        public void Print(string content)
        {
            Console.WriteLine(content);
        }
        public void Send(string message)
        {
            try
            {
                bw.Write(message);
                bw.Flush();
            }catch
            {
                Print("发送失败！");
            }
        }

        public bool Connect()
        {
            try
            {
                client = new TcpClient(remoteHost, remotePort);
                Print("连接成功");
            }
            catch
            {
                throw;
            }
            NetworkStream networkStream = client.GetStream();
            br = new BinaryReader(networkStream);
            bw = new BinaryWriter(networkStream);

            Send("login," + name);


            Thread threadReceive = new Thread(new ThreadStart(ReceiveData));
            threadReceive.IsBackground = true;
            threadReceive.Start();

            return true;
        }
        public void ReceiveData()
        {
            string receiveString = null;
            while (isConnected)
            {
                try
                {
                    receiveString = br.ReadString();
                }
                catch
                {
                    Print("连接已断开");
                    isConnected = false;
                }
                Print(receiveString);
            }
        }

    }
}
