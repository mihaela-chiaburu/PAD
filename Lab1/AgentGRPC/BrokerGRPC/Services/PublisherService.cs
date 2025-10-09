using BrokerGRPC.Models;
using BrokerGRPC.Services.Interfaces;
using Grpc.Core;
using GrpcAgent;

namespace BrokerGRPC.Services
{
    public class PublisherService : Publisher.PublisherBase
    {
        private readonly IMessageStorageService _messageStorage;
        public PublisherService(IMessageStorageService messageStorageService)
        {
            _messageStorage = messageStorageService;
        }
        public override Task<PublishReply> PublishMessage(PublishRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Received message to publish: {request.Topic} {request.Content}");

            var message = new Message(request.Topic, request.Content);
            _messageStorage.Add(message);

            return Task.FromResult(new PublishReply()
            {
                IsSuccess = true
            });
        }
    }
}
