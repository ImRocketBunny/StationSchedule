using MQTTnet;
using MQTTnet.Protocol;
using StationAdvertService.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StationAdvertService.Services
{
    internal class MqttClientService : IMqttClientService
    {
        private IMqttClient? _mqttClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MqttClientService> _logger;
        private ConcurrentDictionary<string, string> _currentBrokerState = new ConcurrentDictionary<string, string>();

        public MqttClientService(IMqttClient? mqttClient, IConfiguration configuration, ILogger<MqttClientService> logger, ConcurrentDictionary<string, string> currentBrokerState)
        {
            _mqttClient = mqttClient;
            _configuration = configuration;
            _logger = logger;
            _currentBrokerState = currentBrokerState;
        }

        private async Task<IMqttClient> InitializeMqttClientAsync(IConfiguration configuration)
        {

            var factory = new MqttClientFactory();
            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer("127.0.0.1", 1883)
                .WithCredentials(Assembly.GetCallingAssembly().GetName().Name, "")
                .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithCleanSession()
                .Build();

            await _mqttClient.ConnectAsync(options);

            return _mqttClient;
        }

        public async Task PublishPlaylist(Dictionary<string, string> keyValuePairs)
        {
            foreach (string key in keyValuePairs.Keys)
            {


                var message = new MqttApplicationMessageBuilder()
                        .WithTopic(_configuration["StationConfiguration:TopicPrefix"] + Path.AltDirectorySeparatorChar + key)
                        .WithPayload(keyValuePairs[key] == "null" ? "{}" : keyValuePairs[key])
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithRetainFlag()
                        .Build();


                _currentBrokerState.TryGetValue(_configuration["StationConfiguration:TopicPrefix"] + Path.AltDirectorySeparatorChar + key, out string value);

                if (value is null || (value != (keyValuePairs[key] == "null" ? "{}" : keyValuePairs[key])))
                    await _mqttClient!.PublishAsync(message);




            }



        }
    }
}
