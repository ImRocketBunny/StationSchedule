using Microsoft.Extensions.DependencyInjection;
using MQTTBrigdeAPI.Services;
using MQTTBrigdeAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddCors();

builder.Services.AddSingleton<IMqttClientService, MqttClientService>();
builder.Services.AddTransient<ILogger>(s => s.GetService<ILogger<Program>>());
builder.Services.AddSingleton<ITaskManagerService, TaskManagerService>();
builder.Services.AddHostedService<Worker>();
//builder.Services.AddScoped<IConfiguration>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   // app.UseSwagger();
   // app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
