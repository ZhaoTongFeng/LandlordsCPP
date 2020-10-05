using LandlordsClient;
using LandlordsServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkTCP
{

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

        public string name;
        public Room room;
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

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="client"></param>
        //连接之后开启一个线程和客户端进行连接，不停处理来自客户端的数据
        public User(TcpClient client)
        {
            this.client = client;
            NetworkStream networkStream = client.GetStream();
            br = new BinaryReader(networkStream);
            bw = new BinaryWriter(networkStream);

            //Task.Run(() => ReceiveFromClient());


            //每一个Controller一个处理函数
            //后面全部放到一个列表中
           
            //实际上并不用开多个线程
            
            Thread thread = new Thread(ReceiveFromClientToServer);
            thread.Start();

        }


        /// <summary>
        /// 向这个用户发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Send(string msg)
        {
            try
            {
                bw.Write(msg);
                bw.Flush();
                return true;
            }
            catch
            {
                return false;
            }
        }


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

        /// <summary>
        /// 处理接受消息
        /// 每一个Controller都有自己的处理方法
        /// </summary>
        public void ReceiveFromClientToServer()
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
                            case "create":onCreateRoom();break;
                            case "join":onJoinRoom(root);break;
                            case "start":onStartGame();break;
                            case "exit":onExitRoom();break;
                            default:Send("msg," + "报头不匹配，无法解析");break;
                        }
                    }
                    else if (package.clsName.Equals("User"))
                    {
                        switch (package.funName)
                        {
                            case "login":onLogin(root);break;
                            case "logout":onLogout();break;
                            default:break;
                        }
                    }
                }
                else
                {

                }
            }
        }
    }
}
