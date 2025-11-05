using Microsoft.Extensions.Options;
using SyncNode.Services;
using SyncNode.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configureaza clasa MovieAPISettings din appsettings.json
builder.Services.Configure<MovieAPISettings>(
    builder.Configuration.GetSection("MovieAPISettings")
);

// Inregistreaza interfata pentru acces usor
builder.Services.AddSingleton<IMovieAPISettings>(sp =>
    sp.GetRequiredService<IOptions<MovieAPISettings>>().Value
);

builder.Services.AddSingleton<SyncWorkJobService>();
builder.Services.AddHostedService<SyncWorkJobService>(sp =>
    sp.GetRequiredService<SyncWorkJobService>()
);

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();
