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
    enum NetState
    {
        Unknown,//未连接
        Connected,//连接成功
        Connecting,//连接中
        DisConnected//连接失败
    }

    class User
    {
        public TcpClient client { get; set; }
        public BinaryReader br { get; set; }
        public BinaryWriter bw { get; set; }
        public string remoteHost { get; set; }

        public int remotePort = 51888;

        public NetState mNetState = NetState.Unknown;

        public bool IsConnecting()
        {
            return mNetState == NetState.Connecting;
        }

        public string name { get; set; }


        public string GetNetStateName()
        {
            switch (mNetState)
            {
                case NetState.Unknown:
                    return "未连接";
                case NetState.Connected:
                    return "连接成功";
                case NetState.Connecting:
                    return "连接中";
                case NetState.DisConnected:
                    return "连接失败";
                default:
                    return "";
            }
        }

        public User()
        {
            remoteHost = Dns.GetHostName();
        }


        public bool Connect()
        {
            mNetState = NetState.Connecting;
            try
            {
                client = new TcpClient(remoteHost, remotePort);
                mNetState = NetState.Connected;
            }
            catch
            {
                mNetState = NetState.DisConnected;
                return false;
            }

            NetworkStream networkStream = client.GetStream();
            br = new BinaryReader(networkStream);
            bw = new BinaryWriter(networkStream);

            Send("login," + name);

            //开启的后台线程处理来自服务器的数据
            Thread threadReceive = new Thread(new ThreadStart(ReceiveData));
            threadReceive.IsBackground = true;
            threadReceive.Start();
            return true;
        }


        public void ReceiveData()
        {
            string receiveString = null;
            while (IsConnecting())
            {
                
                try
                {
                    receiveString = br.ReadString();
                }
                catch
                {
                    mNetState = NetState.DisConnected;
                }

                //这里只是单纯的输出功能
                Print(receiveString);

            }
        }


        /// <summary>
        /// 输出到本地屏幕
        /// </summary>
        /// <param name="content"></param>
        public void Print(string content)
        {
            Console.WriteLine(content);
        }


        /// <summary>
        /// 输出到服务器
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            if (IsConnecting())
            {
                try
                {
                    bw.Write(message);
                    bw.Flush();
                }
                catch
                {
                    mNetState = NetState.DisConnected;
                }
            }
        }
    }
}
