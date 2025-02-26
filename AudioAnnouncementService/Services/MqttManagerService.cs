using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace AudioAnnouncementService.Services
{
    internal class MqttManagerService : IMqttManagerService
    {
        private IMqttClient? _mqttClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IAnnoucementQueueManager _annoucementQueueManager;
        private MqttClientConnectResult? result;
        private Dictionary<string, string> _mqttDataStore;
        private BlockingCollection<string> messageQueue = new BlockingCollection<string>();

        public MqttManagerService(
         IConfiguration configuration, ILogger logger,IAnnoucementQueueManager annoucementQueueManager)
        {
            _configuration = configuration;
            _mqttDataStore = new Dictionary<string, string>();
            _annoucementQueueManager = annoucementQueueManager;
            _logger = logger;
        }


        public async Task SetUpMqttClientAsync()
        {
            if (_mqttClient is null || (_mqttClient is not null && !_mqttClient.IsConnected))
            {

                _mqttClient = await InitializeMqttClientAsync(_configuration);
                SubscribeTopicAsync(_configuration);
                ReceiveNewAnnoucementAsync();
            }

            //_logger.LogInformation($"Mqtt connection Status: {_mqttClient!.IsConnected}");
            
           

        }

        private async Task<IMqttClient> InitializeMqttClientAsync(IConfiguration configuration)
        {

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer("127.0.0.1", 1883)
                .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithCleanSession()
                .Build();

            await _mqttClient.ConnectAsync(options);

            return _mqttClient;
        }

        private async Task SubscribeTopicAsync(IConfiguration configuration)
        {
            List<string> topicList = configuration.GetSection("StationConfiguration:StationStructure").Get<List<string>>()!;
            foreach (var topic in topicList)
            {
                await _mqttClient.SubscribeAsync(
                    topic,
                    MqttQualityOfServiceLevel.AtLeastOnce
                    );


                
                _logger.LogInformation($"MqttClient subscribed to: {topic} ");

            }
        }


        async Task ReceiveNewAnnoucementAsync()
        {
            _mqttClient!.ApplicationMessageReceivedAsync += e =>
            {
                _logger.LogInformation($"Message Received on topic: {e.ApplicationMessage.Topic}");
                if (e.ApplicationMessage.Topic.Contains("delay"))
                {
                    FullCourse[] courses = JsonConvert.DeserializeObject<FullCourse[]>(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment!));
                    if (courses is null)
                    {
                        return Task.CompletedTask;
                    }
                    _annoucementQueueManager.EnqueueDelayAnnoucement(courses);
                    
                }
                else
                {
                    FullCourse course = JsonConvert.DeserializeObject<FullCourse>(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment!));
                    if (course is null)
                    {
                        return Task.CompletedTask;
                    }
                    _annoucementQueueManager.EnqueueTrainAnnoucement(course);
                }
                return Task.CompletedTask;
            };

        }
    }
}
