
namespace StationMQTTBroker
{
    internal class Worker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5000);
        }
    }
}
