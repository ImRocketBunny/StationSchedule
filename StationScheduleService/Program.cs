using MQTTnet.Client;
using StationScheduleService;
using StationScheduleService.Services;

/*var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();*/

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IMqttManagerService, MqttManagerService>();
        services.AddSingleton<ITaskManager, TaskManagerService>();
        services.AddSingleton<IWebScrapperService, WebScrapperService>();
        services.AddSingleton<IPingClientService, PingClientService>();
        services.AddSingleton<IStationScheduleService, StationScheduleService.Services.StationScheduleService>();
        services.AddTransient<ILogger>(s => s.GetService<ILogger<Program>>());
        services.AddHostedService<Worker>();
    })
    //.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration))
    .Build();

host.Run();