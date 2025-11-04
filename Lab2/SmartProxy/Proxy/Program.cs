using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Adauga logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Incarca fisierul ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Adauga servicii Ocelot
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Middleware Ocelot
await app.UseOcelot();

// Endpoint simplu
app.MapGet("/", () => "Smart Proxy");

app.Run();
