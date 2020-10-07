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
        //房号
        public int id { get; set; }
        
        //房名
        public string name { get; set; }

        //房主
        [JsonIgnore]
        public User user { get; set; }

        //玩家列表
        [JsonIgnore]
        public List<User> users { get; set; }

        //房间状态
        
        public RoomState state { get; set; }

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
