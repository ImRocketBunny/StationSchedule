
using StationAPI.Services;

namespace StationAPI
{
    public class Worker : BackgroundService
    {
        private readonly ITaskManagerService _taskManagerService;
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ITaskManagerService taskManagerService, ILogger<Worker> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _taskManagerService = taskManagerService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await WaitUntilStoppedAsync(stoppingToken);
        }

        private async Task WaitUntilStoppedAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
               await _taskManagerService.Execute();
                _logger.LogInformation("Worker is working");
                Thread.Sleep(10000);
            }
        }

    }
}
