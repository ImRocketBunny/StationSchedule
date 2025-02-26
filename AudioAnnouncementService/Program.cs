using AudioAnnouncementService;
using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Services;

/*var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
*/
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IMqttManagerService, MqttManagerService>();
        services.AddSingleton<IAnnoucementQueueManager, AnnoucementQueueManager>();
        services.AddSingleton<IAudioFileService, AudioFileService>();
        services.AddSingleton<ITaskManagerService, TaskManagerService>();
        services.AddSingleton<IAudioService,AudioService>();
        services.AddSingleton<IAudioPlaylistService, AudioPlaylistService>();
        services.AddTransient<ILogger>(s => s.GetService<ILogger<Program>>());
        services.AddHostedService<Worker>();
    })
    //.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration))
    .Build();
host.Run();
