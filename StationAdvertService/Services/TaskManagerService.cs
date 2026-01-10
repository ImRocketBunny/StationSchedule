using StationAdvertService.Abstract;


namespace StationAdvertService.Services
{
    class TaskManagerService : ITaskManagerService
    {
        private readonly IAdvertFileService _advertFileService;
        private readonly IStationAdvertService _advertService;
        private readonly IMqttClientService _mqttClientService;
        public TaskManagerService(IAdvertFileService advertFileService, IStationAdvertService advertService, IMqttClientService mqttClientService)
        {
            _advertFileService = advertFileService;
            _advertService = advertService;
            _mqttClientService = mqttClientService;
        }


        public async Task ExecuteAsync()
        {
            await _mqttClientService.SetUpMqttClientAsync();
            await _advertFileService.ProcessFileManaging();
            _advertService.ProcessAdvertManaging();
        }
    }
}
