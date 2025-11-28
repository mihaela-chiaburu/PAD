using Microsoft.Extensions.Options;
using SyncNode.Services;
using SyncNode.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var movieApiDw1 = Environment.GetEnvironmentVariable("MOVIEAPI_DW1_URL")
    ?? "http://movieapi-instance1/api";
var movieApiDw2 = Environment.GetEnvironmentVariable("MOVIEAPI_DW2_URL")
    ?? "http://movieapi-instance2/api";

builder.Configuration["MovieAPISettings:Hosts:0"] = movieApiDw1;
builder.Configuration["MovieAPISettings:Hosts:1"] = movieApiDw2;

// Log pentru debugging
Console.WriteLine($"Sync Node - MovieAPI DW1: {movieApiDw1}");
Console.WriteLine($"Sync Node - MovieAPI DW2: {movieApiDw2}");

builder.Services.Configure<MovieAPISettings>(
    builder.Configuration.GetSection("MovieAPISettings")
);

builder.Services.AddSingleton<IMovieAPISettings>(sp =>
    sp.GetRequiredService<IOptions<MovieAPISettings>>().Value
);

builder.Services.AddSingleton<SyncWorkJobService>();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();