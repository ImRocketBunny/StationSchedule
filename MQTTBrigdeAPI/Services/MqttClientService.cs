using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.Text;

namespace MQTTBrigdeAPI.Services
{
    public class MqttClientService : IMqttClientService
    {
        private IMqttClient? _mqttClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private MqttClientConnectResult? result;
        private Dictionary<string, string> _mqttDataStore;
        private List<string> _subscriptions;

        public MqttClientService(
         IConfiguration configuration, ILogger logger)
        {

            _configuration = configuration;
            _mqttDataStore = new Dictionary<string, string>();
            _logger = logger;
            _subscriptions = _configuration.GetSection("StationConfiguration:StationStructure").Get<List<string>>();


        }
        public async Task DisposeMqttClientAsync()
        => await _mqttClient.DisconnectAsync();


        public async Task SetUpMqttClientAsync()
        {
           
            if (_mqttClient is null || (_mqttClient is not null && !_mqttClient.IsConnected))
            {
                foreach (var topic in _subscriptions)
                {
                     _mqttDataStore.TryAdd(topic, "{}");
                }

                _mqttClient = await InitializeMqttClientAsync(_configuration);
                await SubscribeTopicAsync();
                await StartStuff();
            }
           
           
        }


        async Task<IMqttClient> InitializeMqttClientAsync(IConfiguration configuration)
        {

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer("127.0.0.1", 1883)
            .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            //.WithCredentials("user","pass")
            .WithCleanSession()
            .Build();

            await _mqttClient.ConnectAsync(options);
            

           

            return _mqttClient;
        }

        async Task SubscribeTopicAsync()
        {

                foreach (var topic in _subscriptions)
                {
                    await _mqttClient.SubscribeAsync(topic);
                }
            
        }



        string GetNextNewestTopicValue(string topic)
        {
            return _mqttDataStore[topic];
        }

        string IMqttClientService.GetNextNewestTopicValue(string topic)
        {
            return _mqttDataStore[topic];
        }

        

        async Task StartStuff()
        {
            _mqttClient!.ApplicationMessageReceivedAsync += e =>
            {
                if (_mqttDataStore.ContainsKey(e.ApplicationMessage.Topic))
                {
                    _mqttDataStore[e.ApplicationMessage.Topic] = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

                }
                else
                {
                    _mqttDataStore.Add(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
                }
                return Task.CompletedTask;
            };
        }
    }
}
