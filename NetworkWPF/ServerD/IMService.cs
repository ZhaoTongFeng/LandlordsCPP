using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkWPF
{
    public class IMService : INetwork
    {
        public void ProcessData(Package package, User sender)
        {
            if (!package.clsName.Equals("IM"))
            {
                return;
            }
            switch (package.funName)
            {
                case "SendMsgToAll":
                    SendMsgToAll(package.data, sender);
                    break;
                case "SendMsgToRoom":
                    SendMsgToRoom(package.data, sender);
                    break;
                case "SendMsgToUser":
                    SendMsgToUser(package.data, sender);
                    break;
                default:
                    break;
            }
        }


        public static void SendMsgToAll(string data, User sender)
        {
            string msg;

            
            if (sender != null)
            {
                msg = sender.name + "：" + data;
                if (sender.state == PlayerState.InRoom)
                {
                    SendMsgToRoom(data, sender);
                }
                else
                {
                    Server.SendToAll(new Package(Package.OPT, "IMPage", "onSendMessage", msg));
                    Server.Message(msg);
                    Server.Log(msg);
                }
            }
            else
            {
                msg = "系统" + "：" + data;
                Server.SendToAll(new Package(Package.OPT, "IMPage", "onSendMessage", msg));
                Server.Message(msg);
                Server.Log(msg);
            }


        }

        public static void SendMsgToRoom(string data, User sender)
        {
            if (sender.room == null)
            {
                Server.Log(sender.name + "不在房间中");
                return;
            }
            string msg = sender.name + ":" + data;
            sender.room.SendToAllClient(new Package(Package.OPT, "IMPage", "onSendMessage", msg));
            Server.Log(msg);
        }

        public static void SendMsgToUser(string data, User sender)
        {

        }
    }
}
