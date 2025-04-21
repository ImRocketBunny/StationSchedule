using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeachUpdateService.Services
{
    internal sealed class MqttManagerService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MqttManagerService> _logger;
        private IMqttClient? _mqttClient;
        private ConcurrentDictionary<string, string> _currentBrokerState;

        public MqttManagerService(
            IConfiguration configuration, ILogger<MqttManagerService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _currentBrokerState= new ConcurrentDictionary<string, string>();
        }


        public async Task SetUpMqttClientAsync()
        {

            if (_mqttClient is null || (_mqttClient is not null && !_mqttClient.IsConnected))
            {


                _mqttClient = await InitializeMqttClientAsync(_configuration);
                //await SubscribeTopicAsync();
                await ReceiveMqttMessageAsync();
            }


        }

        async Task<IMqttClient> InitializeMqttClientAsync(IConfiguration configuration)
        {

            var factory = new MqttClientFactory();
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

        async Task ReceiveMqttMessageAsync()
        {

            _mqttClient!.ApplicationMessageReceivedAsync += e =>
            {
                _currentBrokerState.AddOrUpdate(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.Payload),
                    (key, oldvalue) => Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                return Task.CompletedTask;

            };

        }
    }
}
