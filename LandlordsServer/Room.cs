using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkTCP
{
    public enum RoomState
    {
        Prepare,
        Playing
    }

    /// <summary>
    /// 房间
    /// </summary>
    public class Room
    {
        
        //房间总数
        public static int count = 0;
        
        //房号
        public int id;
        //房名
        public string name = "";
        //房主
        public User user;
        //玩家列表
        public List<User> users;
        //房间状态
        public RoomState state = RoomState.Prepare;

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


        public static Dictionary<String, String> msgs = new Dictionary<string, string>()
        {
            {"create","msg,\"{0}\"创建了房间，ID:{1}" },
            {"join","\"msg,{0}\"加入了房间" },
            {"exit","\"msg,{0}\"退出了房间" },
            {"start","msg,开始游戏" },
            {"shutdown","msg,房间已销毁" }
        };


        public Room()
        {
            users = new List<User>();
        }

        /// <summary>
        /// 给房间中的所有玩家发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendToAllClient(string msg)
        {
            for (int i = 0; i < users.Count; i++)
            {
                users[i].Send(msg);
            }
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="user"></param>
        public void Create(User user) {
            //处理
            id = count++;
            this.user = user;
            name = user.name+"的房间";
            Server.rooms.Add(id, this);
            users.Add(user);

            //通知，这个通知能不能让用户进行调用还是一个问题。如果把这个通知看成一个IM系统的一部分，那么用户肯定是能够调用的。
            Server.SendToAllClient(String.Format(msgs["create"], user.name, id));

            //数据：把这个类序列化发送给客户端
            user.Send("onCreated"+ JsonSerializer.Serialize(this));
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="user"></param>
        public void Join(User user)
        {
            users.Add(user);
            Server.SendToAllClient(String.Format(msgs["join"], user.name));
        }



        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            Server.SendToAllClient(String.Format(msgs["start"]));
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="user"></param>
        public void Exit(User user)
        {
            users.Remove(user);
            Server.SendToAllClient(String.Format(msgs["exit"], user.name));
        }

        /// <summary>
        /// 销毁房间
        /// </summary>
        public void Shutdown()
        {
            count--;
            Server.SendToAllClient(String.Format(msgs["shutdown"]));
        }


    }
}
