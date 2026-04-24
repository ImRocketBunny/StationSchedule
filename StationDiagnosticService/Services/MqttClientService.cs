using MQTTnet;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace StationDiagnosticService.Services
{
    internal class MqttClientService : IMqttClientService
    {
        private IMqttClient? _mqttClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MqttClientService> _logger;
        private readonly IDiagnosticStackManager _diagnosticStackManager;

        public MqttClientService(IConfiguration configuration, ILogger<MqttClientService> logger, IDiagnosticStackManager diagnosticStackManager) 
        {
            _configuration=configuration;
            _logger=logger;
            _diagnosticStackManager = diagnosticStackManager;
        }

        private async Task<IMqttClient> InitializeMqttClientAsync(IConfiguration configuration)
        {

            var factory = new MqttClientFactory();
            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId(_configuration["MqttClient:UserName"])
                .WithTcpServer(_configuration["MqttClient:IP"], int.Parse(_configuration["MqttClient:Port"]!))
                .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithCleanSession()
                .Build();

            await _mqttClient.ConnectAsync(options);

            return _mqttClient;
        }

        public async Task SetUpMqttClientAsync()
        {
            if (_mqttClient is null || (_mqttClient is not null && !_mqttClient.IsConnected))
            {
                _mqttClient = await InitializeMqttClientAsync(_configuration);
                await SubscribeTopicAsync(_configuration);
                ReceiveNewAnnoucementAsync();
                _logger.LogInformation($"MqttClient has been initiated.");
            }
        }



        private async Task SubscribeTopicAsync(IConfiguration configuration)
        {
            List<string> topicList = configuration.GetSection("MqttClient:Topics").Get<List<string>>()!;
            List<string> dataType = configuration.GetSection("MqttClient:TopicsContentType").Get<List<string>>()!;
            string topicPrefix = configuration["MqttClient:TopicPrefix"]!;
            foreach (var contentType in dataType)
            {
                foreach (var topic in topicList)
                {
                    await _mqttClient.SubscribeAsync(
                        topicPrefix+topic+contentType,
                        MqttQualityOfServiceLevel.AtLeastOnce
                    );



                    _logger.LogInformation($"MqttClient subscribed to: {topicPrefix + topic + contentType} ");

                }
            }
            
        }


        async Task ReceiveNewAnnoucementAsync()
        {
            _mqttClient!.ApplicationMessageReceivedAsync += e =>
            {
                _logger.LogInformation($"Message Received on topic: {e.ApplicationMessage.Topic}");
                {
                    DateTime dateTimeReceived = DateTime.Now;
                    string topicReceived = e.ApplicationMessage.Topic;

                    try
                    {
                        using (JsonDocument.Parse(Encoding.UTF8.GetString(e.ApplicationMessage.Payload!)))
                        {
                            //JsonObject mqttDataReceived = JsonConvert.DeserializeObject<JsonObject>(Encoding.UTF8.GetString(e.ApplicationMessage.Payload!));
                            JsonObject mqttDataReceived = JsonNode.Parse(Encoding.UTF8.GetString(e.ApplicationMessage.Payload!))!.AsObject();
                            if (mqttDataReceived != null)
                            {
                                _logger.LogInformation($"Stacking Json data frame");
                                _diagnosticStackManager.PrepareJsonFrame(topicReceived, mqttDataReceived, dateTimeReceived);
                            }
                        }
                    }
                    catch (System.Text.Json.JsonException ex)
                    {
                        string textDataReceived = Encoding.UTF8.GetString(e.ApplicationMessage.Payload!);
                        if(textDataReceived != null)
                        {
                            _logger.LogInformation($"Stacking text data frame");
                            _diagnosticStackManager.PrepareTextFrame(topicReceived, textDataReceived, dateTimeReceived);
                        }
                       
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError($"Error occured during message receive process: {ex.Message} ");
                    }


                   
                }
                return Task.CompletedTask;
            };

        }
    }
}
