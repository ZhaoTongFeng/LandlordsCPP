using NetworkWPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkWPF
{
    public enum RoomState
    {
        Prepare,
        Playing
    }

    /// <summary>
    /// 游戏房间
    /// </summary>
    public class Room
    {
        //房间列表
        public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();

        //房间总数
        public static int count = 0;
        
        //房号
        public int id { get; set; }
        
        //房名
        public string name { get; set; }
        
        //房主
        public User user;

        //玩家列表
        public List<User> users;

        //房间状态
        public RoomState state { get; set; }

        //预制消息模板
        public static Dictionary<String, String> msgs = new Dictionary<string, string>()
        {
            {"create","msg,\"{0}\"创建了房间，ID:{1}" },
            {"join","\"msg,{0}\"加入了房间" },
            {"exit","\"msg,{0}\"退出了房间" },
            {"start","msg,开始游戏" },
            {"shutdown","msg,房间已销毁" }
        };


        //没有公开的构造函数，用反序列化直接构建

        private Room(User user)
        {
            state = RoomState.Prepare;
            this.user = user;
            users = new List<User>();
            users.Add(user);
            id = count++;
            name = user.name + "的房间";
            rooms.Add(id, this);
            user.room = this;
            
            
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="user"></param>
        public static Room Create(User user) {
            Room room = new Room(user);
            //MessageSender.SendToAllUsers(user, String.Format(msgs["create"], user.name, id));
            //数据：把这个类序列化发送给客户端
            //user.Send("onCreated"+ JsonSerializer.Serialize(this));
            return room;
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="user"></param>
        public static bool Join(User user, JsonElement root)
        {
            int id = root.GetProperty("id").GetInt32();

            //这里还没有处理房间已经消失的情况
            Room room;
            try
            {
                Room.rooms.TryGetValue(id, out room);
            }
            catch 
            {

                return false;
            }
            room.users.Add(user);
            user.room = room;

            //MessageSender.SendToRoomUsers(user, String.Format(msgs["join"], user.name));
            return true;
        }



        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            //MessageSender.SendToRoomUsers(user,String.Format(msgs["start"]));
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="user"></param>
        public void Exit()
        {
            rooms.Remove(user.room.id);
            user.room = null;
            users.Remove(user);
            //MessageSender.SendToRoomUsers(user, String.Format(msgs["exit"], user.name));
        }

        /// <summary>
        /// 销毁房间
        /// </summary>
        public void Shutdown()
        {
            count--;
           // MessageSender.SendToRoomUsers(user,String.Format(msgs["shutdown"]));
        }

        public string GetStateName()
        {
            switch (state)
            {
                case RoomState.Prepare:
                    return "准备中";
                case RoomState.Playing:
                    return "游戏中";
                default:
                    return "";
            }
        }


        /// <summary>
        /// 给房间中的所有玩家发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendToAllClient(Package pg)
        {
            for (int i = 0; i < users.Count; i++)
            {
                users[i].Send(pg);
            }
        }
    }
}
