using StationAdvertService;
using StationAdvertService.Abstract;
using StationAdvertService.Services;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IMqttClientService, MqttClientService>();

var host = builder.Build();
host.Run();
