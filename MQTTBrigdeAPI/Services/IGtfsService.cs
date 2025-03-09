using StationAPI.Models;
using TransitRealtime;

namespace StationAPI.Services
{
    public interface IGtfsService
    {
        Task TaskManager();

        Task<List<Vehicle>> UpdateGTFSPositions();
    }
}
