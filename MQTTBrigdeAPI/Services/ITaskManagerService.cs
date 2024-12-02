namespace MQTTBrigdeAPI.Services
{
    public interface ITaskManagerService
    {
        Task Execute();
        string GetNewestValue(string topic);
    }
}
