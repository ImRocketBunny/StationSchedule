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
        private Dictionary<string, string> openWithOlds = new Dictionary<string, string>();


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
            int i = 0;
            foreach (string key in keyValuePairs.Keys)
            {

                /*if (!keyValuePairs[key].Contains("main"))
                {

                    Console.WriteLine(keyValuePairs[key]);
                }*/
                //Console.WriteLine(keyValuePairs.Keys.Count);
                var message = new MqttApplicationMessageBuilder()
                        .WithTopic("station/" + key)
                        .WithPayload(keyValuePairs[key] == "null" ? "{}" : keyValuePairs[key])
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithRetainFlag()
                        .Build();


                /*if (openWithOlds.Keys.Count >0)
                {
                    Console.WriteLine(openWithOlds[key] == keyValuePairs[key]);
                }*/

                if ((openWithOlds.ContainsKey(key) && openWithOlds[key] != keyValuePairs[key]) || !openWithOlds.ContainsKey(key))
                {

                        if (!openWithOlds.ContainsKey(key))
                        {
                        openWithOlds.Add(key, keyValuePairs[key]);
                            Console.WriteLine("Adding key: " + key);
                        }
                        else
                        {
                            Console.WriteLine("Updating key: " + key);
                            openWithOlds[key] = keyValuePairs[key];
                        }
                       // Console.WriteLine(_mqttDataStore.Keys.Count);
                    //Console.WriteLine(_mqttDataStore.ContainsKey(key) && _mqttDataStore[message.Topic.Replace("station/", "")] == (keyValuePairs[key]));
                    await _mqttClient!.PublishAsync(message);
                    //Console.WriteLine("update: "+key);    

                }
                
         
            }
           // openWithOlds = keyValuePairs;

            

        }

    
    }
}
