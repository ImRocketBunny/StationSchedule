using StationAPI.Models;

namespace StationAPI.Abstract.DAL
{
    public interface IApiRepository
    {
        Task<List<string>> GetAdvertPlaylist(int stationId);

        Task<List<Train>> GetGtfsKMPositions();
    }
}
