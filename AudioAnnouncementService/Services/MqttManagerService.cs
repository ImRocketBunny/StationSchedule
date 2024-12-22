using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Models;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Services
{
    internal class MqttManagerService
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


        async Task ReceiveNewAnnoucementAsync()
        {
            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                if (e.ApplicationMessage.Topic.Contains("delay"))
                {
                    FullCourse[] courses = JsonConvert.DeserializeObject<FullCourse[]>(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
                    if (courses == null)
                    {
                        return Task.CompletedTask;
                    }
                    _annoucementQueueManager.EnqueueDelayAnnoucement(courses);
                    
                }
                else
                {
                    _mqttDataStore.Add(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
                }
                messageQueue.Add(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)); // Dodaj wiadomość do kolejki
                return Task.CompletedTask;
            };
        }


    }
}
