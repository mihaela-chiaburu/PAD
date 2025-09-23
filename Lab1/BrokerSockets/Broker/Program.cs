using Helpers;
using System;
using System.Text;

namespace Broker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Broker");

            BrokerSocket brokerSocket = new BrokerSocket();
            brokerSocket.Start(Settings.BROKER_IP, Settings.BROKER_PORT);
            
            var worker = new Worker();
            Task.Factory.StartNew(worker.DoSendMessageWork, TaskCreationOptions.LongRunning);

            Console.ReadLine();
        }
    }
}