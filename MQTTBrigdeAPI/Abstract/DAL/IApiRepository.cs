namespace StationAPI.Abstract.DAL
{
    public interface IApiRepository
    {
        List<string> GetAdvertPlaylist(int stationId);
    }
}
