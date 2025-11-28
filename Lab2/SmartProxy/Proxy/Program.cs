using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Ocelot.Configuration.File;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Railway PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(int.Parse(port)));

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ENV variables
var dw1 = Environment.GetEnvironmentVariable("MOVIEAPI_DW1_HOST");
var dw2 = Environment.GetEnvironmentVariable("MOVIEAPI_DW2_HOST");

// Load Ocelot config
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add Ocelot + CacheManager
builder.Services
    .AddOcelot(builder.Configuration)
    .AddCacheManager(x => x.WithDictionaryHandle());

// Inject ENV dynamically
builder.Services.PostConfigure<FileConfiguration>(cfg =>
{
    if (!string.IsNullOrWhiteSpace(dw1) && !string.IsNullOrWhiteSpace(dw2))
    {
        Console.WriteLine("Using Railway hosts for load balancing:");
        Console.WriteLine($"DW1 = {dw1}");
        Console.WriteLine($"DW2 = {dw2}");

        foreach (var route in cfg.Routes)
        {
            route.DownstreamScheme = "https";

            route.DownstreamHostAndPorts = new List<FileHostAndPort>
            {
                new FileHostAndPort { Host = dw1, Port = 443 },
                new FileHostAndPort { Host = dw2, Port = 443 }
            };

            // Important for POST/PUT/DELETE
            route.HttpHandlerOptions = new FileHttpHandlerOptions
            {
                AllowAutoRedirect = false,
                UseCookieContainer = false,
                UseProxy = false
            };

            // Forward Content-Type correctly
            route.UpstreamHeaderTransform ??= new Dictionary<string, string>();
            route.DownstreamHeaderTransform ??= new Dictionary<string, string>();
            route.UpstreamHeaderTransform["Content-Type"] = "Content-Type";
            route.DownstreamHeaderTransform["Content-Type"] = "Content-Type";
        }
    }
});

var app = builder.Build();

// Must be BEFORE endpoints
await app.UseOcelot();

app.MapGet("/", () => "Smart Proxy is running!");

app.Run();
