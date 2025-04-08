using StationAPI.Models;

namespace StationAPI.Abstract.DAL
{
    public interface IApiRepository
    {
        Task<List<string>> GetAdvertPlaylist(int stationId, int platform);
        Task<List<Train>> GetGtfsKMPositions();
    }
}
