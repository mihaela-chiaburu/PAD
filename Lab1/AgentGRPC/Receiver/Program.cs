using Common;
using Grpc.Net.Client;
using GrpcAgent;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Receiver.Services;

Console.WriteLine("Receiver");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<NotificationService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
_ = app.RunAsync(EndpointsConstants.SubscribersAddress);

await Task.Delay(1000); 

Subscribe(app);

Console.WriteLine("Press enter to exit ");
Console.ReadLine();

static void Subscribe(WebApplication app)
{
    var channel = GrpcChannel.ForAddress(EndpointsConstants.BrokerAddress);
    var client = new Subscriber.SubscriberClient(channel);

    Console.Write("Enter the topic: ");
    var topic = Console.ReadLine().ToLower();

    var address = app.Services.GetRequiredService<IServer>()
        .Features.Get<IServerAddressesFeature>()?.Addresses.First();

    Console.WriteLine($"Subscriber listening at address: {address}");

    var request = new SubscribeRequest
    {
        Topic = topic,
        Address = address,
    };

    try
    {
        var reply = client.Subscribe(request);
        Console.WriteLine("Subscription " + (reply.IsSuccess ? "successful" : "failed"));
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error subscribing : {e.Message}");
    }
}