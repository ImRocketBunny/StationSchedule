using Microsoft.Extensions.DependencyInjection;
using StationAPI.Services;
using StationAPI;
using StationAPI.DAL.Context;
using Microsoft.EntityFrameworkCore;
using StationAPI.Abstract.DAL;
using StationAPI.DAL.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(connectionString)/*, ServiceLifetime.Singleton*/);

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IApiRepository, ApiRepository>();
builder.Services.AddSingleton<IMqttClientService, MqttClientService>();
builder.Services.AddTransient<ILogger>(s => s.GetService<ILogger<Program>>());
builder.Services.AddSingleton<ITaskManagerService, TaskManagerService>();


//builder.Services.AddScoped<IConfiguration>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
