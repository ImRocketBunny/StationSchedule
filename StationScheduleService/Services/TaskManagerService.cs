using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    
    internal sealed class TaskManagerService : ITaskManager
    {
        //private readonly IMqttManagerService _mqttManagerService;
        private readonly ILogger _logger;
        private readonly IStationScheduleService _stationScheduleService;
        private readonly IMqttManagerService _mqttManagerService;

        public TaskManagerService( ILogger logger, IStationScheduleService stationScheduleService, IMqttManagerService mqttManagerService)
        {
           
            _logger = logger;
            _stationScheduleService = stationScheduleService;
            _mqttManagerService = mqttManagerService;
        }

        public async Task Execute()
        {
            try
            {
                await _mqttManagerService.SetUpMqttClientAsync();
                await _stationScheduleService.GetScheduleContent();
            }
            catch (Exception ex) 
            {
                _logger.LogError("Task manager has thrown an exception: {ExMessage}", ex.Message);
            }
            
        }
    }
}
