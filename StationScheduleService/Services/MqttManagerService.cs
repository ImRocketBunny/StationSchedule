using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using StationScheduleService.Models;
using System;
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
        private readonly ILogger _logger;
        private MqttClientConnectResult? result;
        private Dictionary<string, string> _mqttDataStore;


        public MqttManagerService(
         IConfiguration configuration/*, IMqttClient mqttClient*/, ILogger logger)
        {
            
            _configuration = configuration;
            _mqttDataStore = new Dictionary<string, string>();
            //_mqttClient = mqttClient;
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


        public async Task PublishSchedule(Dictionary<string, string> keyValuePairs)
        {
            foreach (string key in keyValuePairs.Keys)
            {
                //Console.WriteLine(key);
                var message = new MqttApplicationMessageBuilder()
                        .WithTopic("station/" + key)
                        .WithPayload(keyValuePairs.GetValueOrDefault(key) == "null" ? "{}" : keyValuePairs.GetValueOrDefault(key))
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithRetainFlag()
                        .Build();
            
                /*if ((_mqttDataStore.ContainsKey(key) && _mqttDataStore[message.Topic.Replace("station/", "")] == (keyValuePairs[key])) || _mqttDataStore.Keys.Count == 0)
                {
                    if (!_mqttDataStore.ContainsKey(key))
                    {
                        _mqttDataStore.Add(message.Topic.Replace("station/",""), Encoding.UTF8.GetString(message.PayloadSegment));
                    }
                    else
                    {
                        _mqttDataStore[message.Topic.Replace("station/", "")] = Encoding.UTF8.GetString(message.PayloadSegment);
                    }*/
                    await _mqttClient!.PublishAsync(message);
                    

                //}
                
         
            }
            //_mqttDataStore = keyValuePairs;



        }

    
    }
}
