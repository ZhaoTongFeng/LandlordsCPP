using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandlordsCS
{
    class Game
    {
        public bool isRunning { get; set; }
        private int i = 0;

        private Stopwatch sw = new Stopwatch();
        private long tickCount = 0, currentTick = 0;
        public static float deltaTime = 0;


        LandlordsGameMode mGameMode;


		//初始化
		public bool Initialize()
        {
            //Do Some Thing
            isRunning = true;
            sw.Start();
            mGameMode = new LandlordsGameMode();
            return true;
        }

		//游戏循环
		public void Loop()
        {
            while (isRunning)
            {
                ProcessInput();
                UpdateGame();
                GenerateOutput();
            }
        }

		//游戏结束
		public void Shutdown()
        {
            //Do Some Thing
        }


        //按键输入
        private void ProcessInput()
        {
            mGameMode.ProcessInput();
        }

		//更新游戏
		private void UpdateGame()
        {
            
            while (sw.ElapsedMilliseconds - tickCount < 512) ;
            currentTick = sw.ElapsedMilliseconds;
            deltaTime = (currentTick - tickCount) / 1000;
            tickCount = currentTick;
            //Do Some Thing
            mGameMode.UpdateGame(deltaTime);
        }

        //输出图像
        private void GenerateOutput()
        {
            Console.Clear();
            Console.WriteLine(mGameMode.GenerateOutput());
        }
	}
}
