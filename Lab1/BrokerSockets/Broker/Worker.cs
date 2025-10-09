using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broker
{
    class Worker
    {
        private const int TIME_TO_SLEEP = 500;

        public void DoSendMessageWork()
        {
            while (true)
            {
                while(!PayloadStorage.IsEmpty())
                {
                    var payload = PayloadStorage.GetNext();
                    if (payload != null)
                    {
                        var connections = ConnectionsStorage.GetSubscribersByTopic(payload.Topic);
                        foreach (var connection in connections)
                        {
                            var payloadString = JsonConvert.SerializeObject(payload);
                            byte[] data = Encoding.UTF8.GetBytes(payloadString);

                            connection.socket.Send(data);
                        }
                    }
                }
                Thread.Sleep(TIME_TO_SLEEP);
            }
        }
    }
}
