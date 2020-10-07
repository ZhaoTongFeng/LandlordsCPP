
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

using System.Text.Json;
using System.Threading;


namespace NetworkWPF
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
        private BinaryReader br;
        private BinaryWriter bw;
        private TcpClient client;

                //关闭
        public void Close()
        {
            br.Close();
            bw.Close();
            client.Close();
        }
        public int id { get; set; }

        public string name { get; set; }
        
        public string password { get; set; }

        public Room room { get; set; }

        public bool isLogin { get; set; }

        //游戏状态
        public PlayerState state { get; set; }

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

        public bool IsConnected()
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


        public User()
        {
            isLogin = false;

        }
        public User(int id, string name, string password)
        {
            this.id = id;
            this.name = name;
            this.password = password;
            isLogin = false;
        }






        /////////////////////////////////////////////////////////////////
        ///连接
        /////////////////////////////////////////////////////////////////
        /// <summary>
        /// 服务端与客户端建立连接
        /// </summary>
        /// <param name="client"></param>
        public bool ConnecToClient(TcpClient client)
        {
            this.client = client;
            NetworkStream networkStream = client.GetStream();
            br = new BinaryReader(networkStream);
            bw = new BinaryWriter(networkStream);
            mNetState = NetState.Connected;
            Thread thread = new Thread(ReceiveData);
            thread.IsBackground = true;
            thread.Start();
            return true;
        }

        /// <summary>
        /// 客户端与服务器建立连接
        /// </summary>
        /// <returns></returns>
        public void ConnectToServer()
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
                return;
            }

            NetworkStream networkStream = client.GetStream();
            br = new BinaryReader(networkStream);
            bw = new BinaryWriter(networkStream);
            mNetState = NetState.Connected;
            //处理来自服务器的数据
            Thread threadReceive = new Thread(new ThreadStart(ReceiveData));
            threadReceive.IsBackground = true;
            threadReceive.Start();
        }


        /////////////////////////////////////////////////////////////////
        ///发送数据
        /////////////////////////////////////////////////////////////////
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="pg">数据包</param>
        public bool Send(Package pg)
        {
            try
            {
                bw.Write(pg.ToString());
                bw.Flush();
                return true;
            }
            catch
            {
                mNetState = NetState.DisConnected;
                return false;
            }
        }

        /////////////////////////////////////////////////////////////////
        ///处理数据
        /////////////////////////////////////////////////////////////////
        /// <summary>
        /// 网络数据回调接口列表
        /// </summary>
        List<INetwork> networks = new List<INetwork>();
        /// <summary>
        /// 注册
        /// 任何需要响应网络数据的类都必须进行注册
        /// </summary>
        /// <param name="cls"></param>
        public void Register(INetwork cls)
        {
            networks.Add(cls);
        }
        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="cls"></param>
        public void UnRegister(INetwork cls)
        {
            networks.Remove(cls);
        }

        /// <summary>
        /// 数据接收线程
        /// 交给注册的类进行处理
        /// </summary>
        private void ReceiveData()
        {
            string receiveString = null;
            while (true)
            {
                try
                {
                    receiveString = br.ReadString();
                }
                catch
                {
                    mNetState = NetState.DisConnected;
                    break;
                }
                
                Package pg = JsonSerializer.Deserialize<Package>(receiveString);
                foreach (INetwork item in networks)
                {
                    item.ProcessData(pg, this);
                }
            }
        }

        /////////////////////////////////////////////////////////////////
        ///客户端
        /////////////////////////////////////////////////////////////////
        public string remoteHost = Dns.GetHostName();

        public int remotePort = 51888;




        /////////////////////////////////////////////////////////////////
        ///服务端
        /////////////////////////////////////////////////////////////////

        /// <summary>
        /// 处理用户提交的登录数据
        /// 传入用户名和密码
        /// 放回登录是否成功
        /// </summary>
        /// <param name="jsonString"></param>
        public void onLogin(JsonElement root)
        {
            name = root.GetProperty("name").ToString();
            //Server.SendToAllClient("msg," + name + "上线了," + "在线人数" + Server.users.Count);
        }


        /// <summary>
        /// 正常下线操作
        /// 从各个列表中清除
        /// </summary>
        public void onLogout()
        {
            DisConnected();
        }




        

        //掉线处理
        public void DisConnected()
        {
            //MessageSender.SendToAllUsers(this,name + "下线了");
            Server.users.Remove(this);

        }
    }
}
