using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LandlordsCS;
using NetworkTCP;

namespace NetworkTCPClient
{
    public enum NetState
    {
        Unknown,//未连接
        Connected,//连接成功
        Connecting,//连接中
        DisConnected//连接失败
    }
    public enum PlayerState
    {
        Online,
        InRoom,
        Playing
    }
    public class User
    {
        public TcpClient client { get; set; }
        public BinaryReader br { get; set; }
        public BinaryWriter bw { get; set; }
        public string remoteHost { get; set; }

        public int remotePort = 51888;

        public string name = "";
        public Room room;

        public User()
        {
            remoteHost = Dns.GetHostName();
        }


        //玩家在线状态
        public PlayerState state = PlayerState.Online;
        public string GetStateName()
        {
            switch (state)
            {
                case PlayerState.Online:
                    return "在线";
                case PlayerState.InRoom:
                    return "等待中";
                case PlayerState.Playing:
                    return "游戏中";
                default:
                    return "";
            }
        }




        //网络状态
        public NetState mNetState = NetState.Unknown;

        public bool IsConnecting()
        {
            return mNetState == NetState.Connected;
        }

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

            //处理来自服务器的数据
            Thread threadReceive = new Thread(new ThreadStart(ReceiveData));
            threadReceive.IsBackground = true;
            threadReceive.Start();

            //再开一个线程用来发送消息
            Thread outThread = new Thread(new ThreadStart(SendData));
            outThread.IsBackground = true;
            outThread.Start();

            return true;
        }

        //接收线程
        //对服务器的指令进行解析，应该使用消息队列的模式，将接收到的消息经过一定解析存放到一个队列，这个队列将明确指出应该做何种处理，在主线程中每帧去处理这个队列，而不是在这个线程中直接对主线程进行操作
        //按道理说是可以用回调的方法去调用，但是如果有很多个GameMode，不可能全部放到这里进行处理，还不如全部放到队列中，让Gamemode自己去检测，
        //不要在主线程之外将消息打印到屏幕
        //Game.Print(receiveString);
        private void ReceiveData()
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
                if (Game.IsProcessInMsgQue)
                {
                    Game.inMsgQueBuffer.Enqueue(receiveString);
                }
                else
                {
                    Game.inMsgQue.Enqueue(receiveString);
                }
                
            }
        }

        //发送线程
        private void SendData()
        {
            while (true)
            {
                if (Game.outMsgQue.Count != 0)
                {
                    string msg = Game.outMsgQue.Dequeue();
                    Send(msg);
                }
            }
        }

        private void Send(string message)
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
