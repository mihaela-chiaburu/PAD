using Helpers;
using System;
using System.Threading;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Subscriber");

            var subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(Settings.BROKER_IP, Settings.BROKER_PORT);

            Thread inputThread = new Thread(() =>
            {
                while (true)
                {
                    Console.Write("Enter a topic to subscribe to (or type 'exit' to quit): ");
                    string input = Console.ReadLine().ToLower().Trim();

                    if (input == "exit")
                        break;

                    if (!string.IsNullOrEmpty(input))
                    {
                        subscriberSocket.Subscribe(input);
                    }
                }

                Console.WriteLine("Closing subscriber...");
            });

            inputThread.IsBackground = true;
            inputThread.Start();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
