using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieAPI.Repositories;
using MovieAPI.Services;
using MovieAPI.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Configureaza clasa MovieAPISettings din appsettings.json
builder.Services.Configure<SyncServicesSettings>(
    builder.Configuration.GetSection("SyncServiceSettings")
);

builder.Services.AddSingleton<ISyncServiceSettings>(sp =>
    sp.GetRequiredService<IOptions<SyncServicesSettings>>().Value
);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISyncService<Movie>, SyncService<Movie>>();

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
