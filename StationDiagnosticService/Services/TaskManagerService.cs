using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationDiagnosticService.Services
{
    internal class TaskManagerService : ITaskManagerService
    {
        private readonly IMqttClientService _mqttClientService;
        private readonly ILogger<TaskManagerService> _logger;
        private readonly IFileService _fileService;
        private readonly IDiagnosticService _diagnosticService;

        public TaskManagerService(IMqttClientService mqttClientService, ILogger<TaskManagerService> logger, IFileService fileService,IDiagnosticService diagnosticService)
        {
            _mqttClientService = mqttClientService;
            _logger = logger;
            _fileService = fileService;
            _diagnosticService = diagnosticService;
           
        }




        public async Task ExecuteAsync()
        {
            await _mqttClientService.SetUpMqttClientAsync();
            await _fileService.StartProcesingFrames();
            await _diagnosticService.ExecuteAsync();
        }


    }
}
