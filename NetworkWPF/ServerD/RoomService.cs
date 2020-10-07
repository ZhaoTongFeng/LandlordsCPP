using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetworkWPF.ServerD
{
    public class RoomService : INetwork
    {

        //房间列表
        public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();

        //房间总数
        public static int count = 0;

        //预制消息模板
        public static Dictionary<String, String> msgs = new Dictionary<string, string>()
        {
            {"create","{0}创建了房间" },
            {"join","{0}加入了房间" },
            {"exit","{0}退出了房间" },
            {"start","游戏开始" },
            {"shutdown","房间已销毁" }
        };


        static RoomService()
        {
            //开个线程不停的告诉闲置状态用户房间信息，
        }


        public void ProcessData(Package package, User sender)
        {
            if (!package.clsName.Equals("Room"))
            {
                return;
            }
            switch (package.funName)
            {
                case "GetList":
                    GetList(package.data, sender);
                    break;
                case "Create":
                    Create(package.data, sender);
                    break;
                case "Join":
                    Join(package.data, sender);
                    break;
                case "Exit":
                    Exit(package.data, sender);
                    break;
                case "Start":
                    StartGame(package.data, sender);
                    break;
                default:
                    break;
            }
        }
        public static void GetList(string data, User sender)
        {
            //在创建和加入退出销毁开始游戏等等动作之后，room的属性或者rooms的数量会变化
            //需要将即时信息发送给所有在线用户。

        }
        public static void Create(string data, User sender)
        {
            //创建一个房间，设置房间状态
            Room room = new Room();
            room.id = rooms.Count;
            room.name = sender.name + "的房间";
            room.user = sender;
            room.users = new List<User>();
            room.users.Add(room.user);
            room.state = RoomState.Prepare;

            sender.room = room;

            //添加到List
            rooms.Add(room.id, room);

            //通知
            IMService.SendMsgToAll(String.Format(msgs["create"], sender.name),sender);
            
            //返回房间信息
            sender.Send(new Package(Package.OPT, "RoomListPage", "onCreatedRoom", JsonSerializer.Serialize(room)));

            Server.UpdateStatus();
        }

        public static void Join(string data, User sender)
        {

            int id = int.Parse(data);
            Room room;
            try
            {
                rooms.TryGetValue(id, out room);
                room.users.Add(sender);
                sender.room = room;
                IMService.SendMsgToRoom(String.Format(msgs["join"],sender.name), sender);
                //返回房间信息
                sender.Send(new Package(Package.OPT, "RoomListPage", "onJoinRoom", JsonSerializer.Serialize(room)));
            }
            catch
            {
                //这里还没有处理房间已经消失的情况
            }
        }


        public void Exit(string data, User sender)
        {

        }

        public void StartGame(string data, User sender)
        {

        }

        public void CloseRoom()
        {
            count--;
            Server.UpdateStatus();
        }
    }
}
