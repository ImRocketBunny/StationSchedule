namespace MQTTBrigdeAPI.Services
{
    public interface IMqttClientService
    {
        Task SetUpMqttClientAsync();
        Task DisposeMqttClientAsync();
        string GetNextNewestTopicValue(string topic);
    }
}
