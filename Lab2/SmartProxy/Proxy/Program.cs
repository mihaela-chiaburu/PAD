using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Ocelot.Configuration.File;

var builder = WebApplication.CreateBuilder(args);

// PORT pentru proxy (Railway) - MOVED UP!
var webPort = Environment.GetEnvironmentVariable("PORT") ?? "8080";
Console.WriteLine($"Proxy will listen on port: {webPort}"); // DEBUG

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(webPort));
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var movieApiDw1Host = Environment.GetEnvironmentVariable("MOVIEAPI_DW1_HOST");
var movieApiDw2Host = Environment.GetEnvironmentVariable("MOVIEAPI_DW2_HOST");

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration)
    .AddCacheManager(x => x.WithDictionaryHandle());

builder.Services.PostConfigure<FileConfiguration>(config =>
{
    if (!string.IsNullOrEmpty(movieApiDw1Host) && !string.IsNullOrEmpty(movieApiDw2Host))
    {
        Console.WriteLine($"Overriding Ocelot config with Railway hosts:");
        Console.WriteLine($"  DW1: {movieApiDw1Host}");
        Console.WriteLine($"  DW2: {movieApiDw2Host}");

        var scheme = movieApiDw1Host.Contains("railway.app") ? "https" : "http";
        var port = scheme == "https" ? 443 : 80;

        foreach (var route in config.Routes)
        {
            route.DownstreamScheme = scheme;
            route.DownstreamHostAndPorts = new List<FileHostAndPort>
            {
                new FileHostAndPort { Host = movieApiDw1Host, Port = port },
                new FileHostAndPort { Host = movieApiDw2Host, Port = port }
            };
        }
    }
    else
    {
        Console.WriteLine("Using default Ocelot config (local development)");
    }
});

var app = builder.Build();

// Middleware Ocelot
await app.UseOcelot();

// Endpoint simplu pentru health check
app.MapGet("/", () => "Smart Proxy is running!");

Console.WriteLine($"Proxy starting...");

app.Run();