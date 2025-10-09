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
            Console.WriteLine($"New cliet trying to subscribe: {request.Address} {request.Topic}");

            try
            {
                var conncetion = new Connection(request.Address, request.Topic);
                _connectionStorage.AddConnection(conncetion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not add the new connection {ex.Message}");
                Console.WriteLine($"Error subscribing client: {request.Address} {request.Topic}");
                return Task.FromResult(new SubscribeReply()
                {
                    IsSuccess = false,
                });
            }

            return Task.FromResult(new SubscribeReply()
            {
                IsSuccess = true,
            });
        }
    }
}
