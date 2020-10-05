using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LandlordsClient;
using LandlordsCS;
using NetworkTCPClient;

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

        public static void ProcessNetworkPackage(Package pg)
        {
            //不会吧MSG传送到这里，所以到这里的全都是OPT
            if (pg.funName.Equals("onCreateRoom"))
            {
                //反序列化，创建房间实例之后，这个room应该是在user类中保存的
                Game.locUser.room = JsonSerializer.Deserialize<Room>(pg.data);
            }
        }

        /// <summary>
        /// 给房间中的所有玩家发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendToAllClient(string msg)
        {
            string data= "msg,room,"+msg;
            Game.SendString(data);
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="user"></param>
        public static void Create() {
            string data = "create";
            Game.SendString(data);
        }

        public void onCreated()
        {

        }



        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="user"></param>
        public static void Join(int i)
        {
            string data = "join," + i;
            Game.SendString(data);
        }

        public void onJoin()
        {

        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            string data = "start";
            Game.SendString(data);
        }

        public void onStartGame()
        {

        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="user"></param>
        public void Exit()
        {
            string data = "exit";
            Game.SendString(data);
        }

        /// <summary>
        /// 销毁房间
        /// </summary>
        public void Shutdown()
        {
            string data = "shutdown";
            Game.SendString(data);
        }
    }
}
