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
    class User
    {
        public BinaryReader br { get; private set; }

        public BinaryWriter bw { get; private set; }

        private string name;

        private TcpClient client;

        public User(TcpClient client)
        {
            this.client = client;
            NetworkStream networkStream = client.GetStream();
            br = new BinaryReader(networkStream);
            bw = new BinaryWriter(networkStream);
            //Task.Run(() => ReceiveFromClient());
            Thread thread = new Thread(ReceiveFromClient);
            thread.Start();
        }
        public void Close()
        {
            br.Close();
            bw.Close();
            client.Close();
        }
        public void ReceiveFromClient()
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
                    Server.RemoveUser(this);
                    return;
                }
                string[] split = receiveString.Split(',');
                switch (split[0])
                {
                    case "login":
                        name = split[1];
                        Server.SendToAllClient(name + "进入了房间," + "在线人数" + Server.users.Count);
                        break;
                    case "logout":
                        Server.SendToAllClient(name + "退出了房间。");
                        Server.RemoveUser(this);
                        break;
                    case "msg":
                        Server.SendToAllClient(name + "：" + receiveString.Remove(0,3));
                        break;
                    default:
                        Server.SendToAllClient("报头不匹配，无法解析");
                        break;
                }
            }
        }
    }
}
