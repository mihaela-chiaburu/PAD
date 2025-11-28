using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieAPI.Repositories;
using MovieAPI.Services;
using MovieAPI.Settings;
using System;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

var webPort = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{webPort}");

var pgHost = Environment.GetEnvironmentVariable("PGHOST");
var pgPort = Environment.GetEnvironmentVariable("PGPORT");  
var pgDatabase = Environment.GetEnvironmentVariable("PGDATABASE");
var pgUser = Environment.GetEnvironmentVariable("PGUSER");
var pgPassword = Environment.GetEnvironmentVariable("PGPASSWORD");

var connectionString = $"Host={pgHost};Port={pgPort};Database={pgDatabase};Username={pgUser};Password={pgPassword}";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.Configure<SyncServicesSettings>(
    builder.Configuration.GetSection("SyncServiceSettings")
);

builder.Services.AddSingleton<ISyncServiceSettings>(sp =>
    sp.GetRequiredService<IOptions<SyncServicesSettings>>().Value
);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISyncService<Movie>, SyncService<Movie>>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
