using BrokerGRPC.Models;
using BrokerGRPC.Services.Interfaces;
using Grpc.Core;
using GrpcAgent;

namespace BrokerGRPC.Services
{
    public class SubscriberService : Subscriber.SubscriberBase
    {
        private readonly IConnectionStorageService _connectionStorage;
        public SubscriberService(IConnectionStorageService connectionStorage)
        {
            _connectionStorage = connectionStorage;
        }

        public override Task<SubscribeReply> Subscribe(SubscribeRequest request, ServerCallContext context)
        {
            var conncetion = new Connection(request.Address, request.Topic);

            _connectionStorage.AddConnection(conncetion);
            Console.WriteLine($"New subscriber with address {request.Address} subscribed to topic {request.Topic}");

            return Task.FromResult(new SubscribeReply()
            {
                IsSuccess = true,
            });
        }
    }
}
