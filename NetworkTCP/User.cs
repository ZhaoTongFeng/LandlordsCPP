using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkTCP
{
    public class User
    {
        public BinaryReader br;

        public BinaryWriter bw;

        public string name;

        private TcpClient client;

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
        /// 处理
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
                    Server.DisConnected(this);
                    return;
                }
                string[] split = receiveString.Split(',');


                Room room;
                int id;
                switch (split[0])
                {
                    case "login":
                        this.name = split[1];
                        Server.SendToAllClient(this.name + "上线了," + "在线人数" + Server.users.Count);
                        break;
                    case "logout":
                        Server.DisConnected(this);
                        break;
                    case "msg":
                        Server.SendToAllClient(this.name + "：" + receiveString.Remove(0, 4));
                        break;

                    //房间
                    case "create":
                        room = new Room();
                        room.Create(this);

                        break;
                    case "join":
                        id = int.Parse(split[1]);
                        room = Server.rooms[id];
                        room.Join(this);

                        break;
                    case "exit":
                        id = int.Parse(split[1]);
                        room = Server.rooms[id];
                        room.Exit(this);
                        break;
                    case "start":
                        id = int.Parse(split[1]);
                        room = Server.rooms[id];
                        room.StartGame();
                        break;


                    default:
                        Server.SendToAllClient("报头不匹配，无法解析");
                        break;
                }
            }
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
