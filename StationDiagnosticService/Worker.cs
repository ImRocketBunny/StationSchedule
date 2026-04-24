using StationDiagnosticService.Services;

namespace StationDiagnosticService
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
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    await _taskManagerService.ExecuteAsync();
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
