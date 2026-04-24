using StationDiagnosticService.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationDiagnosticService.Services
{
    internal class DiagnosticService : IDiagnosticService
    {
        private readonly ILogger<DiagnosticService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDiagnosticStackManager _stackManager;
        private readonly IFileService _fileService;



        private DateTime _timedCheck;
        private int _maxWorkers = 5;


        private readonly List<WorkerInfo> _workers = new();
        private readonly object _lock = new();



        public DiagnosticService(ILogger<DiagnosticService> logger, IConfiguration configuration, IDiagnosticStackManager stackManager, IFileService fileService)
        {
            _logger = logger;
            _configuration = configuration;
            _stackManager = stackManager;
            _fileService = fileService;
            _timedCheck = DateTime.Now;
        }


        public async Task ExecuteAsync()
        {
            if (_timedCheck < DateTime.Now)
            {
                if (_stackManager.GetStackCount() > 10)
                {
                    this.AddWorker();
                }
                await Task.Delay(1000);
                RemoveIdleWorker();
                _timedCheck = _timedCheck.AddMinutes(5);
            }
        }


        public void RemoveIdleWorker()
        {
            lock (_lock)
            {
                var worker = _workers.FirstOrDefault(w =>
                    Interlocked.CompareExchange(ref w.IsBusy, 0, 0) == 0);

                if (worker == null || _workers.Count <= 1)
                    return;

                _workers.Remove(worker);
                worker.Cts.Cancel();
            }
        }


        private async Task WorkerLoop(WorkerInfo worker)
        {
            var token = worker.Cts.Token;
            _logger.LogInformation($"Worker added. Current worker number is {_workers.Count}");

            while (!token.IsCancellationRequested)
            {
                if (_stackManager.TryPopFrame(out DataFrame frame))
                {
                  
                    Interlocked.Exchange(ref worker.IsBusy, 1);

                    try
                    {
                       await ProcessFrameData(frame);
                    }
                    finally
                    {
                       
                        Interlocked.Exchange(ref worker.IsBusy, 0);
                    }
                }
                else
                {
                    Thread.Yield();
                }
            }
        }



        public void AddWorker()
        {
            lock (_lock)
            {
                if (_workers.Count >= _maxWorkers)
                    return;

                var worker = new WorkerInfo
                {
                    Cts = new CancellationTokenSource(),
                    IsBusy = 0
                };

                _workers.Add(worker);

                Task.Run(() => WorkerLoop(worker));

                _logger.LogInformation($"Worker added, current worker count is {_workers.Count}");
            }
        }



        private async Task ProcessFrameData(DataFrame dataFrame)
        {
            try
            {
                await _fileService.LogAsync(dataFrame);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }





        





    }


    
}
