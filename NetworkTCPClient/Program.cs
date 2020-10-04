using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LandlordsCS;

namespace NetworkTCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            if (game.Initialize())
            {
                game.Loop();
            }
            game.Shutdown();
        }
    }
}
