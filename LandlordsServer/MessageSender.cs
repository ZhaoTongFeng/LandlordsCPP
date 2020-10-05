using LandlordsClient;
using NetworkTCP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;




namespace LandlordsServer
{
    //处理

    //通知，这个通知能不能让用户进行调用还是一个问题。如果把这个通知看成一个IM系统的一部分，那么用户肯定是能够调用的。
    public class Message
    {
        //暂时用不到ID
        [JsonIgnore]
        public int id { get; set; }
        public string userName { get; set; }
        public string data { get; set; }

        public Message(string userName, string data)
        {
            this.userName = userName;
            this.data = data;
        }
    }

    class MessageSender
    {
        //消息打包，构造报文，准备传输
        public static Package PackMessage(string msg)
        {
            Package package = new Package();
            package.type = Package.MSG;
            package.data = msg;
            return package;
        }

        //全局广播
        public static void SendToAllUsers(User sender, string msg)
        {
            Message message = new Message(sender.name, msg);
            Package package = PackMessage(JsonSerializer.Serialize(message));
            Server.SendToAllClient(package.ToString());
        }

        //房间广播
        public static void SendToRoomUsers(User sender,string msg)
        {
            Message message = new Message(sender.name, msg);
            Package package = PackMessage(JsonSerializer.Serialize(message));
            sender.room.SendToAllClient(package.ToString());
        }

        //一对一
        public static void SendToUser(User sender, User target,string msg)
        {
            Message message = new Message(sender.name, msg);
            Package package = PackMessage(JsonSerializer.Serialize(message));
            target.Send(package.ToString());
        }
    }
}
