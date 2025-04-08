using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StationAPI.Abstract.DAL;
using StationAPI.DAL.Repository;

namespace StationAPI.Controllers
{
    public class AdvertController : Controller
    {
        private readonly ILogger<MqttBridgeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IApiRepository _apiRepository;
        private readonly IPlaylistRepository _playlistRepository;
        public AdvertController(ILogger<MqttBridgeController> logger, IConfiguration configuration, IApiRepository apiRepository, IPlaylistRepository playlistRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _apiRepository = apiRepository;
            _playlistRepository = playlistRepository;
        }
        [Route("api/advertPlaylist/{stationId}/{platform}")]
        [HttpGet]
        public async Task<IActionResult> GetAdvertPlaylist(int stationId,int platform)
        {

            var adverts = await _apiRepository.GetAdvertPlaylist(stationId,platform);
            return Ok(adverts);

        }
        [HttpPost]
        [Route("app/createPlaylist")]
        public async Task<IActionResult> CreatePlaylist()
        {
            var playlist = Request.ReadFromJsonAsync<JObject>().Result;
            await _playlistRepository.CreatePlaylist(playlist!);
            return Ok();
        }

        [HttpDelete]
        [Route("app/deletePlaylist/{playlistId}")]
        public async Task<IActionResult> DeletePlaylist(int playlistId)
        {
            await _playlistRepository.DeletePlaylist(playlistId!);
            return Ok();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
