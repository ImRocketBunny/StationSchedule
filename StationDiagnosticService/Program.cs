using StationDiagnosticService;
using StationDiagnosticService.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IMqttClientService, MqttClientService>();
builder.Services.AddSingleton<ITaskManagerService, TaskManagerService>();
builder.Services.AddSingleton<IDiagnosticStackManager, DiagnosticStackManager>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<IDiagnosticService, DiagnosticService>();
var host = builder.Build();
host.Run();
