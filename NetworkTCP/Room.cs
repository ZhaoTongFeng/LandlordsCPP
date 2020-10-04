using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkTCP
{


    /// <summary>
    /// 房间
    /// </summary>
    public class Room
    {
        
        public static int count = 0;
        public int id;

        public string name = "";

        public User user;//房主

        public List<User> users;//玩家列表

        public static Dictionary<String, String> msgs = new Dictionary<string, string>()
        {
            {"create","\"{0}\"创建了房间，ID:{1}" },
            {"join","\"{0}\"加入了房间" },
            {"exit","\"{0}\"退出了房间" },
            {"start","开始游戏" },
            {"shutdown","房间已销毁" }
        };


        public Room()
        {
            users = new List<User>();
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="user"></param>
        public void Create(User user) {
            id = count++;
            this.user = user;
            name = user.name+"的房间";
            Server.rooms.Add(id, this);
            Server.SendToAllClient(String.Format(msgs["create"], user.name, id));
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
        /// 离开房间
        /// </summary>
        /// <param name="user"></param>
        public void Exit(User user)
        {
            users.Remove(user);
            Server.SendToAllClient(String.Format(msgs["exit"], user.name));
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            Server.SendToAllClient(String.Format(msgs["start"]));
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
