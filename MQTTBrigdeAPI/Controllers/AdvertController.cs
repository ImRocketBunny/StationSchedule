using Microsoft.AspNetCore.Mvc;
using StationAPI.Abstract.DAL;
using StationAPI.DAL.Repository;

namespace StationAPI.Controllers
{
    public class AdvertController : Controller
    {
        private readonly ILogger<MqttBridgeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IApiRepository _apiRepository;
        public AdvertController(ILogger<MqttBridgeController> logger, IConfiguration configuration, IApiRepository apiRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _apiRepository = apiRepository;
        }
        [Route("api/advertPlaylist/{stationId}")]
        [HttpGet]
        public IActionResult GetAdvertPlaylist(int stationId)
        {

            var adverts = _apiRepository.GetAdvertPlaylist(stationId);
            return Ok(adverts);

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
