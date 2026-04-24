using MediaConverter.Models;
using MediaConverter.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MediaConverter.Services
{
    internal class MediaConversionQueueService : IMediaConversionQueueService,IHostedService, IAsyncDisposable
    {
        private readonly Channel<ConversionJob> _channel = Channel.CreateUnbounded<ConversionJob>();
        private readonly ILogger<MediaConversionQueueService> _logger;
        private readonly List<Task> _workers = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly IConfiguration _configuration;
        private readonly int _workerCount;
        public MediaConversionQueueService(IConfiguration configuration, ILogger<MediaConversionQueueService> logger) 
        {
            _configuration = configuration;
            _logger = logger;
            _workerCount = _configuration.GetValue<int>("MediaConversion:WorkerCount",
                                                    2);
        }

        public Task EnqueueAsync(ConversionJob job)
            => _channel.Writer.WriteAsync(job).AsTask();


        public Task StartAsync(CancellationToken cancellationToken)
        {
            for (int i = 0; i < _workerCount; i++)
                _workers.Add(Task.Run(() => WorkerLoop(_cts.Token)));

            return Task.CompletedTask;
        }

        private async Task WorkerLoop(CancellationToken ct)
        {
            await foreach (var job in _channel.Reader.ReadAllAsync(ct))
            {
                try
                {
                    job.Status = JobStatus.Processing;
                    _logger.LogInformation($"");
                    await job.ExecuteAsync(ct);
                    job.Status = JobStatus.Completed;
                    _logger.LogInformation($"");
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    job.Status = JobStatus.Failed;
                    job.Error = ex;
                    _logger.LogError($"");

                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Writer.Complete();
            _cts.Cancel();
            await Task.WhenAll(_workers);
        }

        public async ValueTask DisposeAsync()
        {
            _cts.Cancel();
            _channel.Writer.TryComplete();
            await Task.WhenAll(_workers);
        }

    }
}
