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


        public void SendMsgToAll(string data, User sender)
        {
            string msg = sender.name + ":" + data;
            Server.SendToAll(new Package(Package.OPT, "IMPage", "onSendMessage",msg));
            Server.Message(msg);
        }

        public void SendMsgToRoom(string data, User sender)
        {
            string msg = sender.name + ":" + data;
            sender.room.SendToAllClient(new Package(Package.OPT, "IMPage", "onSendMessage", msg));
        }
        public void SendMsgToUser(string data, User sender)
        {

        }
    }
}
