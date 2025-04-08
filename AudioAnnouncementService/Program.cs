using AudioAnnouncementService;
using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Builders;
using AudioAnnouncementService.Services;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddSingleton<IMqttManagerService, MqttManagerService>();
builder.Services.AddSingleton<IAnnoucementQueueManager, AnnoucementQueueManager>();
builder.Services.AddSingleton<IAudioFileService, AudioFileService>();
builder.Services.AddSingleton<ITaskManagerService, TaskManagerService>();
builder.Services.AddSingleton<IAudioService, AudioService>();
builder.Services.AddSingleton<IAudioPlaylistService, AudioPlaylistService>();
builder.Services.AddTransient<ILogger>(s => s.GetService<ILogger<Program>>());
builder.Services.AddTransient<TrainAnnoucementBuilder>();
builder.Services.AddHostedService<Worker>();


var host = builder.Build();
host.Run();

