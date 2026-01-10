using MediaConverter.Services.Abstract;

namespace MediaConverter
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
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await _taskManagerService.ExecuteTask();
                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}
