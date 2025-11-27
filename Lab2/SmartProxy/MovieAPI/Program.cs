using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieAPI.Repositories;
using MovieAPI.Services;
using MovieAPI.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.UseAuthorization();
app.MapControllers();
app.Run();
