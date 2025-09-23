using Helpers;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Publisher");

            var publisherSocket = new PublisherSocket();
            publisherSocket.Connect(Settings.BROKER_IP, Settings.BROKER_PORT);

            if (publisherSocket.isConnected)
            {
                while (true)
                {
                    var payload = new Payload();
                    Console.Write("Enter Topic: ");
                    payload.Topic = Console.ReadLine().ToLower();
                    Console.Write("Enter Message: ");
                    payload.Message = Console.ReadLine();

                    var payloadString = JsonConvert.SerializeObject(payload);
                    byte[] payloadData = Encoding.UTF8.GetBytes(payloadString);

                    publisherSocket.Send(payloadData);
                }
            }

            Console.ReadLine();
        }
    }
}