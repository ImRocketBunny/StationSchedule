using MQTTnet.Server;

namespace StationMQTTBroker.Abstract
{
    public interface IMqttBrokerService
    {
        void AddMqttHandlers(MqttServer mqttServer);
    }
}
