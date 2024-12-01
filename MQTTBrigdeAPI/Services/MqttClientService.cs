using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace MQTTBrigdeAPI.Services
{
    public class MqttClientService : IMqttClientService
    {
        private IMqttClient? _mqttClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private MqttClientConnectResult? result;
        private Dictionary<string, string> _mqttDataStore;
        private Dictionary<string, string> openWithOlds = new Dictionary<string, string>();


        public MqttClientService(
         IConfiguration configuration/*, IMqttClient mqttClient*/, ILogger logger)
        {

            _configuration = configuration;
            _mqttDataStore = new Dictionary<string, string>();
            //_mqttClient = mqttClient;
            _logger = logger;
        }
        public async Task DisposeMqttClientAsync()
        => await _mqttClient.DisconnectAsync();


        public async Task SetUpMqttClientAsync()
        {
            if (_mqttClient is null || (_mqttClient is not null && !_mqttClient.IsConnected))
            {

                _mqttClient = await InitializeMqttClientAsync(_configuration);
                await SubscribeTopicAsync(_configuration);
            }


        }


        private async Task<IMqttClient> InitializeMqttClientAsync(IConfiguration configuration)
        {

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer("127.0.0.1", 1883)
            //.WithCredentials("user","pass")
            .WithCleanSession()
            .Build();

            await _mqttClient.ConnectAsync(options);
            //_logger.LogInformation(_mqttClient.IsConnected.ToString());
            return _mqttClient;
        }

        private async Task SubscribeTopicAsync(IConfiguration configuration)
        {
            List<string> topicList = new List<string>();
            foreach (var topic in topicList)
            {
                await _mqttClient.SubscribeAsync(topic);
            }
        }



        public Task GetNextCourseToAnnouce(object value)
        {
            throw new NotImplementedException();
        }
    }
}
