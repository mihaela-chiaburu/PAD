using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broker
{
    public class PayloadHandler
    {
        public static void Handle(byte[] payloadBytes, ConnectionInfo connectionInfo)
        {
            var payloadString = Encoding.UTF8.GetString(payloadBytes);

            if (payloadString.StartsWith("subscribe#"))
            {
                string topic = payloadString.Split("subscribe#")[1];
                connectionInfo.Topics.Add(topic); 
                ConnectionsStorage.AddOrUpdate(connectionInfo, topic); 
                Console.WriteLine($"Subscriber {connectionInfo.Adress} subscribed to topic: {topic}");
            }
            else
            {
                Payload payload = JsonConvert.DeserializeObject<Payload>(payloadString);
                PayloadStorage.Add(payload);
            }

            Console.WriteLine(payloadString);
        }
    }
}
