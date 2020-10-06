using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LandlordsClient;
using NetworkTCP;
using NetworkTCPClient;

namespace LandlordsCS
{
    public class Game
    {

        ///////////////////////////////////////////////////////////////////////////////////
        ///游戏基本系统
        ///////////////////////////////////////////////////////////////////////////////////
        public bool isRunning { get; set; }
        private Stopwatch sw = new Stopwatch();
        private long tickCount = 0, currentTick = 0;
        public static float deltaTime = 0;

        //游戏模式
        LandlordsGameMode mGameMode;


        /// <summary>
        /// 初始化基本程序环境
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            //Do Some Thing
            isRunning = true;
            sw.Start();

            //加载游戏模块（不一定在游戏开始的时候就把所有模块加载，玩家可以进行选择，当然加载了后面直接调用也是没有问题的）
            mGameMode = new LandlordsGameMode();

            return true;
        }


        /// <summary>
        /// 游戏主循环
        /// </summary>
        public void Loop()
        {
            while (isRunning)
            {
                ProcessInput();
                UpdateGame();
                GenerateOutput();
            }
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        public void Shutdown()
        {
            //Do Some Thing
            //这里表示客户端直接关闭了，属于正常关闭，应该礼貌的通知服务器，我下线了，而不是让服务器自己去判断某个用户是否下线了，当然，如果没有提供断线重连等待的服务，那么客户端通不通知就无所谓了

            //像碰到网络延迟或者丢包，或者掉线导致服务器和客户端断开连接，不应该直接判定玩家失败，应该等待一定时间。
        }


        /// <summary>
        /// 处理输入
        /// </summary>
        private void ProcessInput()
        {
            //处理网络输入
            ProcessNetworkInput();

            //处理本地输入
            ProcessLocalInput();
        }


        /// <summary>
        /// 更新游戏状态
        /// </summary>
        private void UpdateGame()
        {

            //输入用户名称

            if (Game.locUser.name.Equals(""))
            {
                Console.WriteLine("用户名：");
                string content = Console.ReadLine();

                if (content.Length != 0)
                {
                    Game.locUser.name = content;

                    if (!Game.locUser.Connect())
                    {
                        return;
                    }
                }
            }

            while (sw.ElapsedMilliseconds - tickCount < 512) ;
            currentTick = sw.ElapsedMilliseconds;
            deltaTime = (currentTick - tickCount) / 1000;
            tickCount = currentTick;

            if (mGameMode.isActivited)
            {
                mGameMode.UpdateGame(deltaTime);
            }
        }



        ///输出
        //在屏幕上显示数据，实际上是将一次迭代中的所有字符串拼接之后，最后一次性输出，而是每次都去调用输出
        //也是个状态机
        public string displayContent = "";

        public void Display(string msg)
        {
            displayContent += msg + "\n";
        }

        public string netMsg = "";

        /// <summary>
        /// 输出游戏状态
        /// </summary>
        private void GenerateOutput()
        {
            Console.Clear();
            //来自网络的输出
            GenerateNetworkOutput();
            //来自本地的输出
            GenerateLocalOutput();

            Console.WriteLine(displayContent);
            displayContent = "";
        }


        ////////////////////////////////////////////////////////////////////////////////////
        ///网络数据接口
        ///////////////////////////////////////////////////////////////////////////////////
        //本地用户
        //1.作为和服务器数据交互的接口
        public static User locUser = new User();


        //网络消息接收队列
        //因为每个层次都有可能对这个队列进行处理，处理之后要进行删除，说是队列，其实整体上并不是先进后出，但是每个模块在处理的时候，都是按照队列进行处理的，所以用一个链表更合适
        public static volatile Queue<String> inMsgQue = new Queue<string>();
        //中间做个缓存，确保在处理消息时，不会发生更改
        public static volatile Queue<String> inMsgQueBuffer = new Queue<string>();
        public static volatile bool IsProcessInMsgQue = false;

        //网络消息发送队列
        //发送线程会不停的检测是否需要发送，一旦有需要发送的信息，就立即按照顺序进行发送，而不管是从那个模块发送的信息，所以是一个严格的队列模型
        public static volatile Queue<String> outMsgQue = new Queue<string>();


        /// <summary>
        /// 发消息给服务器
        /// 实际上是将字符串添加到消息队列，由消息线程将消息进行发送。
        /// </summary>
        public static void SendString(string msg)
        {
            Game.outMsgQue.Enqueue(msg);
        }



        /// <summary>
        /// 处理消息队列
        /// 或者叫处理网络输入
        /// 实际上就是将网络数据解析之后，执行和本地按键输入类似的事件
        /// </summary>
        private void ProcessNetworkInput()
        {
            ///解决方案：双缓冲区，任何时候，总有一个能接收数据
            //冲突:如果在处理的同时，消息线程也在添加，增加了一个缓存又增加了一个锁
            //如果正在处理则会将消息添加到缓冲，否则添加到队列
            //这个锁不会发生死锁，因为是直接读取的内存
            while (inMsgQueBuffer.Count != 0)
            {
                inMsgQue.Enqueue(inMsgQueBuffer.Dequeue());
            }

            IsProcessInMsgQue = true;
            string source;
            while (inMsgQue.Count != 0)
            {
                source = inMsgQue.Dequeue();
                Package pg = new Package(source);
                if (pg.type.Equals(Package.MSG))
                {
                    netMsg = pg.data;
                }
                else if (pg.type.Equals(Package.OPT))
                {
                    //传送给类
                    //这里是手动传送给模块
                    //其实可以在类被初始化的时候在Game进行注册，在这儿进行遍历
                    if (pg.clsName.Equals("Game"))
                    {
                        ProcessNetworkPackage(pg);
                    }
                    else if (pg.clsName.Equals("Room"))
                    {
                        Room.ProcessNetworkPackage(pg);
                    }
                    else if (pg.clsName.Equals("GameMode"))
                    {

                    }
                    else
                    {

                    }
                }
                else
                {

                }
                

                //如果把所有模块都放到for循环里面，一次迭代只遍历一次，一条消息有可能被处理多次
            }
            IsProcessInMsgQue = false;


            //如果放到外面，每次处理之后都会从队列中删除消息，所以一条消息只能被一个模块处理
            //如何实现只遍历一次，但是又可以被多个模块处理？解决方法是这里加一个解析模块，根据消息处理出各个模块需要的数据
            //添加到每个模块的网络数据接收队列，然后轮到某个模块进行处理的时候，再进行处理，有序
            //现在为了简单，直接在一个循环里面同时进行处理
        }


        /// <summary>
        /// 网络数据解析接口
        /// 每一个GameMode都应该有一个，和处理本地输入类似
        /// </summary>
        private void ProcessNetworkPackage(Package pg)
        {

        }

        /// <summary>
        /// 网络数据输出
        /// 在可视化的游戏中，不用单独输出来自网络传输的数据，直接转化为本地的状态进行输出
        /// </summary>
        private void GenerateNetworkOutput()
        {
            Display(netMsg);
            Display(String.Format("连接状态：{0}", Game.locUser.GetNetStateName()));
        }

        ///////////////////////////////////////////////////////////////////////////////////
        ///本地数据接口
        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 本地输入
        /// </summary>
        private void ProcessLocalInput()
        {
            if (mGameMode.isActivited)
            {
                //选择游戏之后，处理该模式下的输入
                mGameMode.ProcessInput();
            }
            else
            {

            }
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo info = Console.ReadKey(true);
                //创建房间
                if (info.Key == ConsoleKey.C)
                {

                    //点击创建房间，通知服务器创建一个房间，服务器将返回房间信息

                }
            }
        }

        /// <summary>
        /// 本地输出
        /// </summary>
        private void GenerateLocalOutput()
        {
            if (mGameMode.isActivited)
            {
                Display(mGameMode.GenerateOutput());
            }
            else
            {
                string str = "";
                str += "C：创建房间\t";

                Display(str);
            }
        }
    }
}
