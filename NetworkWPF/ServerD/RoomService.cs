using LandlordsCS;
using NetworkWPF.Public;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkWPF.ServerD
{
    public class RoomService : INetwork
    {

        //房间列表
        public static volatile Dictionary<string, Room> rooms = new Dictionary<string, Room>();

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
            Thread thread = new Thread(DelayTest);
            thread.IsBackground = true;
            thread.Start();
        }


        //延迟测试
        private static long lastTime = 0;
        public static void DelayTest()
        {
            while (Server.isRunning)
            {
                long cur = Server.GetMilTime();

                if (cur - lastTime > 1000)
                {
                    lastTime = cur;
                    long timeStamp = Util.GetTimeStamp();

                    Server.SendToAll(new Package(Package.OPT, "RoomPage", "onTestDelay", timeStamp.ToString()));
                }
            }
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
                case "Prepare":
                    Prepare(package.data, sender);
                    break;



                case "Exit":
                    Exit(package.data, sender);
                    break;
                case "Start":
                    StartGame(package.data, sender);
                    break;
                case "ProcessInput":
                    ProcessInput(package.data, sender);
                    break;


                case "Remove":
                    Remove(package.data, sender);
                    break;
                default:
                    break;
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        ///游戏大厅操作
        public static void GetList(string data, User sender)
        {
            sender.Send(new Package(Package.OPT, "RoomListPage", "onGetList", JsonSerializer.Serialize(rooms)));
        }

        public static void Create(string data, User sender)
        {
            if (sender.room != null)
            {
                Server.Log(sender.name+ "已经创建或加入了一个房间");
                return;
            }
            //设置房间状态
            Room room = new Room(new LandlordsGameMode());
            room.id = rooms.Count;

            room.Create(sender);

            //添加到List
            rooms.Add(room.id.ToString(), room);

            //返回跳转房间
            sender.Send(new Package(Package.OPT, "RoomListPage", "onCreatedRoom", JsonSerializer.Serialize(room)));
            
            //返回房间内人员信息
            sender.Send(new Package(Package.OPT, "RoomPage", "onUpdate", JsonSerializer.Serialize(room.users)));
            
            //通知所有玩家状态变化
            Server.SendToAll(new Package(Package.OPT, "RoomListPage", "onInsert", JsonSerializer.Serialize(room)));





            IMService.SendMsgToAll(String.Format(msgs["create"], sender.name), sender);
            Server.UpdateStatus();
        }

        public static void Join(string data, User sender)
        {
            if(sender.room != null)
            {
                Server.Log(sender.name + "已经创建或加入了一个房间");
                return;
            }
            int id = int.Parse(data);
            Room room = null;
            if (!rooms.ContainsKey(id.ToString()))
            {
                Server.Log(id + "房间不存在");
                return;
            }
            room = rooms[id.ToString()];
            if (room.IsFull())
            {
                Server.Log(sender.name + "房间已满");
                return;
            }
            room.Enter(sender);

            //跳转房间
            sender.Send(new Package(Package.OPT, "RoomListPage", "onJoinRoom", JsonSerializer.Serialize(room)));

            //返回房间内人员信息
            sender.room.SendToAllClient(new Package(Package.OPT, "RoomPage", "onUpdate", JsonSerializer.Serialize(room.users)));

            //通知所有玩家房间状态变化
            Server.SendToAll(new Package(Package.OPT, "RoomListPage", "onUpdate", JsonSerializer.Serialize(room)));



            IMService.SendMsgToRoom(String.Format(msgs["join"], sender.name), sender);
        }






        //////////////////////////////////////////////////////////////////////////////
        ///房间内操作

        
        public void Prepare(string data, User sender)
        {
            sender.room.Prepare(sender);

            sender.Send(new Package(Package.OPT, "RoomPage", "onPrepare", ""));

            IMService.SendMsgToRoom(sender.name+"准备就绪", sender);

            //自动开始游戏:如果全部玩家都已经准备则开始游戏
            if (sender.room.IsAllPrepare())
            {
                sender.room.SendToAllClient(new Package(Package.OPT, "RoomPage", "onStartGame", ""));
                sender.room.StartGame(sender);
                IMService.SendMsgToRoom("游戏开始", sender);
                for(int i=0;i< sender.room.users.Count; i++)
                {
                    ///!!!
                    ///要想Gamemode能够处理客户端的数据，这里进行注册

                    sender.room.users[i].Register(sender.room.GameMode);
                }
            }
            //返回房间内人员信息
            sender.room.SendToAllClient(new Package(Package.OPT, "RoomPage", "onUpdate", JsonSerializer.Serialize(sender.room.users)));

        }








        public void Exit(string data, User sender)
        {
            //返回房间内人员信息
            sender.room.SendToAllClient(new Package(Package.OPT, "RoomPage", "onUpdate", JsonSerializer.Serialize(sender.room.users)));
        }

        public void StartGame(string data, User sender)
        {
            sender.room.StartGame(sender);

            //返回房间内人员信息
            sender.room.SendToAllClient(new Package(Package.OPT, "RoomPage", "onUpdate", JsonSerializer.Serialize(sender.room.users)));
        }


        public void ProcessInput(string data, User sender)
        {
            //返回各种信息，user已经传入了函数，在函数中直接调用即可
            sender.room.ProcessInput(data, sender);
        }


        public void Remove(string data, User sender)
        {
            count--;

            Room room = sender.room;

            //返回房间信息
            sender.Send(new Package(Package.OPT, "RoomListPage", "onJoinRoom", room.id.ToString()));

            Server.UpdateStatus();

        }
    }
}
