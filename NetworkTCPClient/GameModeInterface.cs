using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandlordsCS
{
    interface GameModeInterface
    {
        /// <summary>
        /// 处理输入
        /// </summary>
        void ProcessInput();

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="deltaTime"></param>
        void UpdateGame(float deltaTime);

        /// <summary>
        /// 文字输出
        /// </summary>
        /// <param name="str"></param>
        string GenerateOutput();

        /// <summary>
        /// 重启游戏
        /// </summary>
        void ReStart();
        
        /// <summary>
        /// 结算奖励
        /// </summary>
        void Settle();

    }
}
