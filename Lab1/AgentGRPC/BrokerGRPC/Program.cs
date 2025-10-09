using BrokerGRPC.Services;
using BrokerGRPC.Services.Interfaces;
using Common;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddGrpc();
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
builder.Services.AddSingleton<IMessageStorageService, MessageStorageService>();
builder.Services.AddSingleton<IConnectionStorageService, ConnectionStorageService>();
builder.Services.AddHostedService<SenderWorker>();

var app = builder.Build();

// Configure middleware and endpoints
app.MapGrpcService<PublisherService>();
app.MapGrpcService<SubscriberService>();
app.MapGet("/", () => "Use a gRPC client to connect to this server.");

app.Run(EndpointsConstants.BrokerAddress);
//trebuie sa asiguram securitatea canalelor pentru nota maxima

