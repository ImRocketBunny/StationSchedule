using MQTTnet;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using StationAdvertService.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StationAdvertService.Services
{
    public class MqttClientService : IMqttClientService
    {
        private IMqttClient? _mqttClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MqttClientService> _logger;
        private ConcurrentDictionary<string, string> _currentBrokerState = new ConcurrentDictionary<string, string>();
        List<string> topics =
["PLK_wylamiane_rogatki_nowe-r20250123-7.webm", "POK_skm_CZARODZIEJSKI-FLET-DLA-DZIECI_03.2025-r20250205-9.webm",
  "Praca_SKM_elektryk_1920x810-r20250116-5.webm", "TS_Mahagonny_1920x810-r20241204-3.webm"
  , "4_UTK_animacja_BAGAZ_NEW-r20250113-5.webm","ZTM_Warszawa_mruga_9.02-r20250203-1.webm","POK_VENUS-AND-ADONIS_25.02-r20250115-3.webm"
,"TS_STARA-1920x810-r20241017-19.webm","ZTM_PLAKAT_E_HOLOGRAM_DLA_HB_TABOR_SKM_2-r20250117-9.webm","SKM_20lecie_1920x810-r20240510-15.webm" ];

        public MqttClientService(IConfiguration configuration, ILogger<MqttClientService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SetUpMqttClientAsync()
        {
            if (_mqttClient is null || (_mqttClient is not null && !_mqttClient.IsConnected))
            {

                _mqttClient = await InitializeMqttClientAsync(_configuration);
                await SubscribeTopicAsync(_configuration);
                ReceiveMqttMessageAsync();
            }


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

        public async Task PublishPlaylist(string payload)
        {
            //foreach (string key in keyValuePairs.Keys)
            //{

                var message = new MqttApplicationMessageBuilder()
                        .WithTopic(_configuration["StationConfiguration:TopicPrefix"] + Path.AltDirectorySeparatorChar + "adverts")
                        .WithPayload(JsonConvert.SerializeObject(topics, Formatting.Indented))
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithRetainFlag()
                        .Build();


                //_currentBrokerState.TryGetValue(_configuration["StationConfiguration:TopicPrefix"] + Path.AltDirectorySeparatorChar + key, out string value);

                //if (value is null || (value != (keyValuePairs[key] == "null" ? "{}" : keyValuePairs[key])))
                    await _mqttClient!.PublishAsync(message);




            //}


            



        }



        public async Task PublishValue(string payload)
        {
            //foreach (string key in keyValuePairs.Keys)
            //{

            var message = new MqttApplicationMessageBuilder()
                    .WithTopic(_configuration["StationConfiguration:TopicPrefix"] + Path.AltDirectorySeparatorChar + "fileName")
                    .WithPayload(JsonConvert.SerializeObject(topics, Formatting.Indented))
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .WithRetainFlag()
                    .Build();


            //_currentBrokerState.TryGetValue(_configuration["StationConfiguration:TopicPrefix"] + Path.AltDirectorySeparatorChar + key, out string value);

            //if (value is null || (value != (keyValuePairs[key] == "null" ? "{}" : keyValuePairs[key])))
            await _mqttClient!.PublishAsync(message);




            //}






        }


        async Task ReceiveMqttMessageAsync()
        {

            _mqttClient!.ApplicationMessageReceivedAsync += e =>
            {
                _currentBrokerState.AddOrUpdate(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.Payload),
                    (key, oldvalue) => Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                //Console.WriteLine($"{e.ApplicationMessage.Topic} {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                return Task.CompletedTask;

            };

        }

        private async Task SubscribeTopicAsync(IConfiguration configuration)
        {
            List<string> topicList = configuration.GetSection("StationConfiguration:StationStructure").Get<List<string>>()!;
            foreach (var topic in topicList)
            {
                await _mqttClient.SubscribeAsync(
                     "station" + Path.AltDirectorySeparatorChar + topic + Path.AltDirectorySeparatorChar + "lcd",
                    MqttQualityOfServiceLevel.AtLeastOnce
                    );



                _logger.LogInformation($"MqttClient subscribed to: {"station" + Path.AltDirectorySeparatorChar + topic + Path.AltDirectorySeparatorChar + "lcd"} ");

            }
        }


        public string? GetCurrentBrokerValue(string key)
        {
            _currentBrokerState.TryGetValue(key, out string? currVal);
            return currVal;
        } 





        public bool IsAnnoucement(string platform)
         => this.GetCurrentBrokerValue(platform) == "{}";
        


        public async Task PublishNumber(int number)
        {
            //foreach (string key in keyValuePairs.Keys)
            //{

            var message = new MqttApplicationMessageBuilder()
                    .WithTopic(_configuration["StationConfiguration:TopicPrefix"] + Path.AltDirectorySeparatorChar + "number")
                    .WithPayload(number.ToString())
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .WithRetainFlag()
                    .Build();


            //_currentBrokerState.TryGetValue(_configuration["StationConfiguration:TopicPrefix"] + Path.AltDirectorySeparatorChar + key, out string value);

            //if (value is null || (value != (keyValuePairs[key] == "null" ? "{}" : keyValuePairs[key])))
            await _mqttClient!.PublishAsync(message);




            //}






        }


        public async Task PublishPlaylist(List<string> playlist, string topic)
        {
            var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(JsonConvert.SerializeObject(playlist, Formatting.Indented))
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .WithRetainFlag()
                    .Build();

            await _mqttClient!.PublishAsync(message);
        }



        public async Task PublishNumber(int number, string platform)
        {
            //foreach (string key in keyValuePairs.Keys)
            //{

            var message = new MqttApplicationMessageBuilder()
                    .WithTopic(platform)
                    .WithPayload(number.ToString())
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .WithRetainFlag()
                    .Build();


            //_currentBrokerState.TryGetValue(_configuration["StationConfiguration:TopicPrefix"] + Path.AltDirectorySeparatorChar + key, out string value);

            //if (value is null || (value != (keyValuePairs[key] == "null" ? "{}" : keyValuePairs[key])))
            await _mqttClient!.PublishAsync(message);




            //}






        }



        public async Task PublishValue(string value, string platform)
        {


            var message = new MqttApplicationMessageBuilder()
                    .WithTopic(platform)
                    .WithPayload(value)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .WithRetainFlag()
                    .Build();



            await _mqttClient!.PublishAsync(message);










        }



    }
}
