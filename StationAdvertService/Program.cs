using StationAdvertService;
using StationAdvertService.Abstract;
using StationAdvertService.Services;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IMqttClientService, MqttClientService>();
builder.Services.AddSingleton<IHttpClientService, HttpClientService>();
builder.Services.AddSingleton<IAdvertFileService, AdvertFileService>();
builder.Services.AddSingleton<ITaskManagerService, TaskManagerService>();
builder.Services.AddSingleton<IStationAdvertService, StationAdvertService.Services.StationAdvertService>();

var host = builder.Build();
host.Run();
