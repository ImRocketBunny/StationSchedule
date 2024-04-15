using ScheduleUpdater;
using ScheduleUpdater.Services;


var host = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<ITaskManagerService, TaskManagerService>();
    }).Build();
host.Run();
