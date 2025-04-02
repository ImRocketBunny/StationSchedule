using Newtonsoft.Json.Linq;
using StationAPI.Abstract.DAL;

namespace StationAPI.DAL.Repository
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ILogger<PlaylistRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        //private readonly DbContext _context;
        public PlaylistRepository(ILogger<PlaylistRepository> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public async Task CreatePlaylist(JObject o)
        {
            throw new NotImplementedException();
        }

        public Task DeletePlaylist(int playlistId)
        {
            throw new NotImplementedException();
        }
    }
}
