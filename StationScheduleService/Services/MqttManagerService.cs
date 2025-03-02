using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using PuppeteerSharp.Input;
using StationScheduleService.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
            
                var factory = new MqttFactory();
                _mqttClient = factory.CreateMqttClient();
                var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(_configuration["MQTTConnectionConfiguration:MQTTServerHost"], int.Parse(_configuration["MQTTConnectionConfiguration:MQTTServerPort"]!))
                .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                //.WithCredentials("user","pass")
                .WithCleanSession()
                .Build();

                await _mqttClient.ConnectAsync(options);
                return _mqttClient;
        }

        private async Task SubscribeTopicAsync(IConfiguration configuration)
        {
            //List<string> topicList = configuration.GetSection("StationConfiguration:StationStructure").Get<List<string>>()!;
            //foreach (var topic in topicList)
            //{
                await _mqttClient.SubscribeAsync("#");
                //await _mqttClient.SubscribeAsync("station/" + topic + "/lcd");
            //}
        }


        public async Task PublishSchedule(Dictionary<string, string> keyValuePairs)
        {
            foreach (string key in keyValuePairs.Keys)
            {

                
                var message = new MqttApplicationMessageBuilder()
                        .WithTopic("station/" + key)
                        .WithPayload(keyValuePairs[key] == "null" ? "{}" : keyValuePairs[key])
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithRetainFlag()
                        .Build();


                    _currentBrokerState.TryGetValue("station/" + key, out string value);
                        
                    if (value is null || (value != (keyValuePairs[key] == "null" ? "{}" : keyValuePairs[key])))
                        await _mqttClient!.PublishAsync(message);

                /*if ((openWithOlds.ContainsKey(key) && openWithOlds[key] != keyValuePairs[key]) || !openWithOlds.ContainsKey(key))
                {
                 

                    if (!openWithOlds.ContainsKey(key))
                    {
                       openWithOlds.Add(key, keyValuePairs[key]);
                       _logger.LogInformation("Adding key: " + key);
                    }
                   else
                   {
                       _logger.LogInformation("Updating key: " + key);
                       openWithOlds[key] = keyValuePairs[key];
                   }

                    await _mqttClient!.PublishAsync(message);
  

                }*/
                
         
            }

            

        }

        Task ReceiveNewAnnoucementAsync()
        {
            
                _mqttClient!.ApplicationMessageReceivedAsync += e =>
            {
                _currentBrokerState.AddOrUpdate(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment),
                    (key, oldvalue) => Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
                return Task.CompletedTask;

            };
            return Task.CompletedTask;

        }


    }
}
