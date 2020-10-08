using LandlordsCS;
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
        //房间人数上限
        public static int MAX_COUNT = 3;

        public bool IsFull()
        {
            return count >= MAX_COUNT;
        }

        //房号
        public int id { get; set; }

        //当前人数
        public int count { get; set; }

        //已准备人数
        public int prepareCount { get; set; }
        
        public bool IsAllPrepare()
        {
            return prepareCount >= MAX_COUNT;
        }

        //房名
        public string name { get; set; }

        //房主
        [JsonIgnore]
        public User user { get; set; }



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
        //玩家列表
        [JsonIgnore]
        public List<User> users { get; set; }



        public Room()
        {

        }
        public Room(GameModeInterface mode)
        {
            GameMode = mode;
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



        public void Create(User sender)
        {
            users = new List<User>();
            state = RoomState.Prepare;
            count = 0;
            prepareCount = 0;

            name = sender.name + "的房间";
            user = sender;

            Enter(sender);
        }

        public void Enter(User sender)
        {
            sender.state = PlayerState.InRoom;
            users.Add(sender);
            sender.room = this;
            sender.isPrepare = false;
            count++;
        }

        public void Prepare(User sender)
        {
            sender.isPrepare = true;
            prepareCount++;
        }

        public void Exit(User sender)
        {
            count--;
            sender.state = PlayerState.Online;
        }



        //游戏部分

        GameModeInterface GameMode;

        public void StartGame(User sender)
        {
            GameMode.ReStart(sender);
        }

        public void ProcessInput(string data, User sender)
        {
            GameMode.ProcessInput(data, sender);
            GameMode.UpdateGame(data, sender);
            GameMode.GenerateOutput(data, sender);
            
        }
    }
}
