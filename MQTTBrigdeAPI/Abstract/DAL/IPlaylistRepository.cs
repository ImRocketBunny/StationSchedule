using Newtonsoft.Json.Linq;

namespace StationAPI.Abstract.DAL
{
    public interface IPlaylistRepository
    {
        Task CreatePlaylist(JObject o);

        Task DeletePlaylist(int playlistId);

    }
}
