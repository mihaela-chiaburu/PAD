using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieAPI.Repositories;
using MovieAPI.Services;
using MovieAPI.Settings;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
var user = Environment.GetEnvironmentVariable("POSTGRES_USER");
var pass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

var connectionString = $"Host={host};Port=5432;Database={database};Username={user};Password={pass}";

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
