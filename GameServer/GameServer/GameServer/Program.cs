using System;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Game Server";

            Server.Start(0, 26950);

            Console.ReadKey();
        }
    }
}
