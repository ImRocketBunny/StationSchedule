using Microsoft.AspNetCore.Mvc;

namespace StationAPI.Controllers
{
    public class GtfsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
