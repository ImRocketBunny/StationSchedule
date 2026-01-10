using MediaConverter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using MediaConverter.Services;
using MediaConverter.Services.Abstract;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ICloudConvertService,CloudConvertService>();
builder.Services.AddSingleton<IFFmpegConversionService, FFmpegConversionService>();
builder.Services.AddSingleton<ITaskManagerService, TaskManagerService>();
builder.Services.AddHttpClient("cloudconvert", (serviceProvider, client) =>
{
    client.DefaultRequestHeaders.Add("Authorization", "Bearer API_KEY");
    client.DefaultRequestHeaders.Add("Content-type", "application/json");

    //client.DefaultRequestHeaders.Add("User-Agent", settings.UserAgent);

    client.BaseAddress = new Uri("https://api.cloudconvert.com/");
});


var host = builder.Build();
host.Run();
