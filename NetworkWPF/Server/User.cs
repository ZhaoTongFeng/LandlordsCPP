using NetworkWPF;
using NetworkWPF;
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

        /////////////////////////////////////////////////////////////////
        ///公共属性
        /////////////////////////////////////////////////////////////////
        private BinaryReader br;
        private BinaryWriter bw;
        private TcpClient client;


        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }

        public Room room { get; set; }

        //游戏状态
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

        }
        public User(int id, string name, string password)
        {
            this.id = id;
            this.name = name;
            this.password = password;
        }


        /////////////////////////////////////////////////////////////////
        ///连接
        /////////////////////////////////////////////////////////////////
        /// <summary>
        /// 与客户端建立连接
        /// 连接之后开启一个线程和客户端进行连接，不停处理来自客户端的数据
        /// </summary>
        /// <param name="client"></param>
        public User(TcpClient client)
        {
            this.client = client;
            NetworkStream networkStream = client.GetStream();
            br = new BinaryReader(networkStream);
            bw = new BinaryWriter(networkStream);

            Thread thread = new Thread(_ReceiveFromClient);
            thread.Start();
        }
        /// <summary>
        /// 与服务器建立连接
        /// </summary>
        /// <returns></returns>
        public bool ConnectToServer()
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
            Thread threadReceive = new Thread(new ThreadStart(_ReceiveDataFromServerThread));
            threadReceive.IsBackground = true;
            threadReceive.Start();

            //再开一个线程用来发送消息
            Thread outThread = new Thread(new ThreadStart(_SendThread));
            outThread.IsBackground = true;
            outThread.Start();

            return true;
        }


        /////////////////////////////////////////////////////////////////
        ///发送数据
        /////////////////////////////////////////////////////////////////
        /// <summary>
        /// 网络消息发送队列
        /// 实现异步发送
        /// 发送线程会不停的检测是否需要发送，一旦有需要发送的信息，就立即按照顺序进行发送，而不管是从那个模块发送的信息，所以是一个严格的队列模型
        /// </summary>
        private Queue<string> outMsgQue = new Queue<string>();


        /// <summary>
        /// 发送线程
        /// 将发送队列中的数据发送
        /// </summary>
        private void _SendThread()
        {
            while (IsConnected())
            {
                if (outMsgQue.Count != 0)
                {
                    string msg = outMsgQue.Dequeue();
                    try
                    {
                        bw.Write(msg);
                        bw.Flush();
                    }
                    catch
                    {
                        mNetState = NetState.DisConnected;
                        break;
                    }

                }
            }
        }

        /// <summary>
        /// 发送
        /// 统一发送，服务端和客户端发送方法完全相同
        /// </summary>
        /// <param name="data"></param>
        public void Send(string data)
        {
            outMsgQue.Enqueue(data);
        }

        /////////////////////////////////////////////////////////////////
        ///处理数据
        /////////////////////////////////////////////////////////////////
        /// <summary>
        /// 客户端接收线程
        /// </summary>
        //对服务器的指令进行解析，应该使用消息队列的模式，将接收到的消息经过一定解析存放到一个队列，这个队列将明确指出应该做何种处理，在主线程中每帧去处理这个队列，而不是在这个线程中直接对主线程进行操作
        //按道理说是可以用回调的方法去调用，但是如果有很多个GameMode，不可能全部放到这里进行处理，还不如全部放到队列中，让Gamemode自己去检测，
        //不要在主线程之外将消息打印到屏幕
        //Game.Print(receiveString);
        private void _ReceiveDataFromServerThread()
        {
            string receiveString = null;
            while (IsConnected())
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

                string source = receiveString;
                Package pg = new Package(source);


                if (pg.type.Equals(Package.MSG))
                {
                    netMsg = pg.data;
                }
                else if (pg.type.Equals(Package.OPT))
                {
                    //传送给类
                    //这里是手动传送给模块
                    //其实可以在类被初始化的时候在Game进行注册，在这儿进行遍历
                    if (pg.clsName.Equals("Game"))
                    {
                        ProcessNetworkPackage(pg);
                    }
                    else if (pg.clsName.Equals("Room"))
                    {
                        Room.ProcessNetworkPackage(pg);
                    }
                    else if (pg.clsName.Equals("GameMode"))
                    {

                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
        }
        /// <summary>
        /// 服务端端接收数据
        /// 每一个Controller都有自己的处理方法
        /// </summary>
        public void _ReceiveFromClient()
        {
            while (true)
            {
                string receiveString = null;
                try
                {
                    receiveString = br.ReadString();
                }
                catch
                {
                    this.DisConnected();
                    return;
                }

                Package package = new Package(receiveString);
                JsonElement root = JsonDocument.Parse(package.data).RootElement;

                //暂时先将所有类的处理放到这儿
                if (package.type == null)
                {

                }
                else if (package.type.Equals(Package.MSG))
                {
                    MessageSender.SendToAllUsers(this, package.data);
                }
                else if (package.type.Equals(Package.OPT))
                {
                    if (package.clsName.Equals("Room"))
                    {
                        switch (package.funName)
                        {
                            case "create": onCreateRoom(); break;
                            case "join": onJoinRoom(root); break;
                            case "start": onStartGame(); break;
                            case "exit": onExitRoom(); break;
                            default: Send("msg," + "报头不匹配，无法解析"); break;
                        }
                    }
                    else if (package.clsName.Equals("User"))
                    {
                        switch (package.funName)
                        {
                            case "login": onLogin(root); break;
                            case "logout": onLogout(); break;
                            default: break;
                        }
                    }
                }
                else
                {

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
            Server.SendToAllClient("msg," + name + "上线了," + "在线人数" + Server.users.Count);
        }


        /// <summary>
        /// 正常下线操作
        /// 从各个列表中清除
        /// </summary>
        public void onLogout()
        {
            DisConnected();
        }

        //创建房间
        public void onCreateRoom()
        {
            Room.Create(this);
        }

        //加入房间
        public void onJoinRoom(JsonElement root)
        {
            Room.Join(this, root);
        }

        //开始游戏
        public void onStartGame()
        {
            room.StartGame();
        }

        //退出房间
        public void onExitRoom()
        {
            if (room != null)
            {
                room.Exit();
            }
        }


        //掉线处理
        public void DisConnected()
        {
            MessageSender.SendToAllUsers(this,name + "下线了");
            Server.users.Remove(this);
        }

        //关闭
        public void Close()
        {
            br.Close();
            bw.Close();
            client.Close();
        }





    }
}
