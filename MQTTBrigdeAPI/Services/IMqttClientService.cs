namespace MQTTBrigdeAPI.Services
{
    public interface IMqttClientService
    {
        Task SetUpMqttClientAsync();
        Task DisposeMqttClientAsync();
        Task GetNextCourseToAnnouce(object value);
    }
}
