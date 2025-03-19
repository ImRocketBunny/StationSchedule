using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StationAPI.Abstract.DAL;
using StationAPI.Services;

namespace StationAPI.Controllers
{
    public class GtfsController : Controller
    {
        private readonly ILogger<GtfsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IGtfsService _gtfsService;
        private readonly IApiRepository _apiRepository;

        public GtfsController(ILogger<GtfsController> logger, IConfiguration configuration, IGtfsService gtfsService, IApiRepository apiRepository) 
        {
            _configuration = configuration;
            _gtfsService = gtfsService;
            _logger = logger;
            _apiRepository = apiRepository;
        }

        [Route("api/gtfsrt")]
        [HttpGet]
        public IActionResult GetGtfsPositions()
        {

            var vehicles = _gtfsService.UpdateGTFSPositions().Result;
            return Ok(JsonConvert.SerializeObject((vehicles), Formatting.Indented));

        }
        [Route("api/gtfskmpositions")]
        [HttpGet]
        public IActionResult GetGtfsKMPositions()
        {

            var vehicles = _apiRepository.GetGtfsKMPositions().Result;
            return Ok(JsonConvert.SerializeObject((vehicles), Formatting.Indented));

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
