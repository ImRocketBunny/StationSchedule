using MQTTnet.Server;
using StationMQTTBroker.Abstract;
using System.Text;

namespace StationMQTTBroker.Services
{

    internal class MqttBrokerService : IMqttBrokerService
    {
        private readonly ILogger<MqttBrokerService> _logger;
        public MqttBrokerService(ILogger<MqttBrokerService> logger)
        {
            _logger = logger;
        }
        public void AddMqttHandlers(MqttServer mqttServer)
        {
            mqttServer.ClientConnectedAsync += ClientConnect;
            mqttServer.ClientDisconnectedAsync += ClientDisconnect;
            mqttServer.ValidatingConnectionAsync += ClientValidate;
            mqttServer.InterceptingPublishAsync += Server_InterceptingPublishAsync;
        }

        private Task ClientConnect(ClientConnectedEventArgs eventArgs)
        {
            _logger.LogInformation($"Client with ClientId: '{eventArgs.ClientId}' and IP: '{eventArgs.Endpoint}' connected.");
            return Task.CompletedTask;
        }
        private Task ClientValidate(ValidatingConnectionEventArgs arg)
        {
            _logger.LogInformation(arg.ClientId, arg.UserName);
            return Task.CompletedTask;
        }
        private Task ClientDisconnect(ClientDisconnectedEventArgs eventArgs)
        {
            return Task.CompletedTask;
        }

        private Task Server_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
  
            _logger.LogInformation(
                " TimeStamp: {0} -- Message: ClientId = {1}, Topic = {2}, QoS = {3}, Retain-Flag = {4}",

                DateTime.Now,
                arg.ClientId,
                arg.ApplicationMessage?.Topic,
                //payload,
                arg.ApplicationMessage?.QualityOfServiceLevel,
                arg.ApplicationMessage?.Retain);
            return Task.CompletedTask;
        }
    }
}
