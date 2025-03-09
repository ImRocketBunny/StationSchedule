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
        public GtfsController(ILogger<GtfsController> logger, IConfiguration configuration, IGtfsService gtfsService) 
        {
            _configuration = configuration;
            _gtfsService = gtfsService;
            _logger = logger;
        }

        [Route("api/gtfsrt")]
        [HttpGet]
        public IActionResult GetGtfsPositions()
        {

            var vehicles = _gtfsService.UpdateGTFSPositions().Result;
            return Ok(JsonConvert.SerializeObject((vehicles), Formatting.Indented));

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
