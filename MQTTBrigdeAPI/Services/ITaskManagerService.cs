namespace StationAPI.Services
{
    public interface ITaskManagerService
    {
        Task Execute();
        string GetNewestValue(string topic);
    }
}
