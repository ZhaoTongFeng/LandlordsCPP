using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            User user = new User();
            string content = Console.ReadLine();
            if (content.Length != 0)
            {
                user.name = content;
            }
            user.Connect();
            while (true)
            {
                content = Console.ReadLine();
                if (content.Length != 0)
                {
                    user.Send(content);
                }

            }
        }
    }
}
