using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ScheduleUpdater
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
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
                await Task.Delay(1000);

            }

        }

    }
}
