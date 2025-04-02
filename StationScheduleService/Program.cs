using MQTTnet.Client;
using StationScheduleService;
using StationScheduleService.DAL.Context;
using StationScheduleService.Services;
using Microsoft.EntityFrameworkCore;
using StationScheduleService.DAL.Abstract;
using StationScheduleService.DAL.Repository;


var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StationDbContext>(options =>
    options.UseSqlServer(connectionString));



builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IMqttManagerService, MqttManagerService>();
builder.Services.AddSingleton<ITaskManager, TaskManagerService>();
builder.Services.AddSingleton<IWebScrapperService, WebScrapperService>();
builder.Services.AddSingleton<IPingClientService, PingClientService>();
builder.Services.AddSingleton<IStationScheduleService, StationScheduleService.Services.StationScheduleService>();
builder.Services.AddTransient<ILogger>(s => s.GetService<ILogger<Program>>());
builder.Services.AddSingleton<IStationRepository, StationRepository>();

builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();

/*var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IMqttManagerService, MqttManagerService>();
        services.AddSingleton<ITaskManager, TaskManagerService>();
        services.AddSingleton<IWebScrapperService, WebScrapperService>();
        services.AddSingleton<IPingClientService, PingClientService>();
        services.AddSingleton<IStationScheduleService, StationScheduleService.Services.StationScheduleService>();
        services.AddTransient<ILogger>(s => s.GetService<ILogger<Program>>());
        services.AddDbContext<StationDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddHostedService<Worker>();
    })
    //.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration))
    .Build();


host.Run();*/