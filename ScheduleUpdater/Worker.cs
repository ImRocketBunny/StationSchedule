using Newtonsoft.Json.Linq;
using ScheduleUpdater.Services;
using System.Net;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ScheduleUpdater
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITaskManagerService _taskManagerService;
        public Worker(ILogger<Worker> logger, ITaskManagerService taskManagerService)
        {
            _logger = logger;
            _taskManagerService = taskManagerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Schedule updater service is starting...");
            await WaitUntilStoppedAsync(stoppingToken);
        }

        private async Task WaitUntilStoppedAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                await _taskManagerService.Execute();
                await Task.Delay(1000);

            }

        }

    }
}
