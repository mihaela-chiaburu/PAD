using Microsoft.Extensions.Options;
using SyncNode.Services;
using SyncNode.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
