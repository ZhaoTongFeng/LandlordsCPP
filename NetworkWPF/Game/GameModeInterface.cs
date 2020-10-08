using NetworkWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandlordsCS
{
    public interface GameModeInterface
    {
        /// <summary>
        /// 重启游戏
        /// </summary>
        void ReStart(User sender);


        /// <summary>
        /// 处理输入
        /// </summary>
        void ProcessInput(string data, User sender);


        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="deltaTime"></param>
        void UpdateGame(string data, User sender);


        /// <summary>
        /// 文字输出
        /// </summary>
        /// <param name="str"></param>
        string GenerateOutput(string data, User sender);


        /// <summary>
        /// 结算奖励
        /// </summary>
        void Settle();

    }
}
