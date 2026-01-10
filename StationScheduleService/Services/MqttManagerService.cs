using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using PuppeteerSharp.Input;
using StationScheduleService.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal class MqttManagerService : IMqttManagerService
    {
        private IMqttClient? _mqttClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MqttManagerService> _logger;
        private Dictionary<string, string> openWithOlds = new Dictionary<string, string>();
        private ConcurrentDictionary<string, string> _currentBrokerState = new ConcurrentDictionary<string, string>();



        public MqttManagerService(
         IConfiguration configuration, ILogger<MqttManagerService> logger)
        {
            _configuration = configuration;
            _logger= logger;
        }
        public async Task DisposeMqttClientAsync()
        => await _mqttClient.DisconnectAsync();


        public async Task SetUpMqttClientAsync()
        {
            if (_mqttClient is null || (_mqttClient is not null && !_mqttClient.IsConnected))
            {
                
                _mqttClient = await InitializeMqttClientAsync(_configuration);
                await SubscribeTopicAsync(_configuration);
                await ReceiveNewAnnoucementAsync();
            }

            
        }


        private async Task<IMqttClient> InitializeMqttClientAsync(IConfiguration configuration)
        {
            
                var factory = new MqttClientFactory();
                _mqttClient = factory.CreateMqttClient();
                var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(_configuration["MQTTConnectionConfiguration:MQTTServerHost"], int.Parse(_configuration["MQTTConnectionConfiguration:MQTTServerPort"]!))
                .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithCredentials(Assembly.GetCallingAssembly().GetName().Name,"pass")
                .WithCleanSession()
                .Build();

                await _mqttClient.ConnectAsync(options);
                return _mqttClient;
        }

        private async Task SubscribeTopicAsync(IConfiguration configuration)
        {
                await _mqttClient.SubscribeAsync("#");
        }


        public async Task PublishSchedule(Dictionary<string, string> scheduleData)
        {
            foreach (string platform in scheduleData.Keys)
            {

                
                var message = new MqttApplicationMessageBuilder()
                        .WithTopic(_configuration["StationConfiguration:TopicPrefix"] + Path.AltDirectorySeparatorChar + platform)
                        .WithPayload(scheduleData[platform] == "null" ? "{}" : scheduleData[platform])
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithRetainFlag()
                        .Build();


                    _currentBrokerState.TryGetValue(_configuration["StationConfiguration:TopicPrefix"] + Path.AltDirectorySeparatorChar + platform, out string value);
                        
                    if (value is null || (value != (scheduleData[platform] == "null" ? "{}" : scheduleData[platform])))
                        await _mqttClient!.PublishAsync(message);

                
                
         
            }

            

        }

        public async Task PublishStationData(Dictionary<string, string> stationData)
        {
            foreach (string data in stationData.Keys)
            {


                var message = new MqttApplicationMessageBuilder()
                        .WithTopic(_configuration["StationConfiguration:TopicPrefix"] + Path.AltDirectorySeparatorChar + data)
                        .WithPayload(stationData[data] == "null" ? "{}" : stationData[data])
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithRetainFlag()
                        .Build();

                await _mqttClient!.PublishAsync(message);




            }
        }

        Task ReceiveNewAnnoucementAsync()
        {
            
                _mqttClient!.ApplicationMessageReceivedAsync += e =>
            {
                _currentBrokerState.AddOrUpdate(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.Payload),
                    (key, oldvalue) => Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                return Task.CompletedTask;

            };
            return Task.CompletedTask;

        }


    }
}
