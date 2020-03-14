using System;
using StackExchange.Redis;

namespace Receiver
{

    class Program
    {

        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("192.168.0.61,allowAdmin=true");
        private const string ChatChannel = "Redis-Channel";
        private static string userName = string.Empty;

        static void Main(string[] args)
        {
            Console.Write("Enter your name: ");
            userName = Console.ReadLine();
            var pubsub = connection.GetSubscriber();
            pubsub.Subscribe(ChatChannel, (channel, message) => MessageAction(message));
            pubsub.Publish(ChatChannel, $"'{userName}' joined the chat room.");
            while (true)
            {
                pubsub.Publish(ChatChannel, $"{userName}: {Console.ReadLine()} ({DateTime.Now.Hour}:{DateTime.Now.Minute})");
            }
        }

        private static void MessageAction(RedisValue message)
        {
            int initialCursorTop = Console.CursorTop;
            int initialCursorLeft = Console.CursorLeft;
            Console.MoveBufferArea(0, initialCursorTop, Console.WindowWidth, 1, 0, initialCursorTop + 1);
            Console.CursorTop = initialCursorTop;
            Console.CursorLeft = 0;
            Console.WriteLine(message);
            Console.CursorTop = initialCursorTop + 1;
            Console.CursorLeft = initialCursorLeft;
        }

    }

}