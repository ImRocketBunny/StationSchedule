using ScheduleMQTTBroker;

var builder = Host.CreateApplicationBuilder(args);
//var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
