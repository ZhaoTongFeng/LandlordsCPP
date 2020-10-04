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
            Thread thread = new Thread(ReceiveFromClient);
            thread.Start();
        }

        /// <summary>
        /// 处理
        /// 每一个Controller都有自己的处理方法
        /// </summary>
        public void ReceiveFromClient()
        {
            Server.ProcessReceive(ref br, this);

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
