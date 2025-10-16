
using Common;
using Grpc.Net.Client;
using GrpcAgent;

Console.WriteLine("Sender");

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
var channel = GrpcChannel.ForAddress(EndpointsConstants.BrokerAddress);
var client = new Publisher.PublisherClient(channel);

while (true)
{
    Console.Write("Enter topic: ");
    var topic = Console.ReadLine().ToLower();
   
    Console.Write("Enter message content: ");
    var content = Console.ReadLine();

    var request = new PublishRequest
    {
        Topic = topic ?? string.Empty,
        Content = content ?? string.Empty
    };
    try
    {
        var reply = await client.PublishMessageAsync(request);
        if (reply.IsSuccess)
        {
            Console.WriteLine("Message published successfully.");
        }
        else
        {
            Console.WriteLine("Failed to publish message.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error publishing message: {ex.Message}");
    }
}