


namespace StationAPI.Services
{
    public class TaskManagerService : ITaskManagerService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private IMqttClientService _mqttClientService;
        public TaskManagerService(ILogger logger, IConfiguration configuration,IMqttClientService mqttClientService) 
        {
            _logger = logger;
            _configuration = configuration;
            _mqttClientService = mqttClientService;
        }
        



        public async Task Execute()
        {
            try
            {
                await _mqttClientService.SetUpMqttClientAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Task manager has thrown an exception: {ExMessage}", ex.Message);
            }

        }

        public string GetNewestValue(string topic)
        {
            return _mqttClientService.GetNextNewestTopicValue(topic);
        }

       
    }
}
