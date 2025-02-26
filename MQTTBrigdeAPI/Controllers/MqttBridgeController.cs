using Microsoft.AspNetCore.Mvc;
using StationAPI.Models;
using StationAPI.Services;
using Newtonsoft.Json;


namespace StationAPI.Controllers
{
    public class MqttBridgeController : Controller
    {
        private readonly ILogger<MqttBridgeController> _logger;
        private readonly IConfiguration _configuration;
        private ITaskManagerService _taskManagerService;
        public MqttBridgeController(ILogger<MqttBridgeController> logger, IConfiguration configuration,ITaskManagerService taskManagerService) 
        {
            _logger = logger;
            _configuration = configuration;
            _taskManagerService = taskManagerService;
        }
        // GET: GetMqttValueController
        public ActionResult Index()
        {
            return View();
        }


        [Route("api/course")]
        [HttpPost]
        public async Task<IActionResult> GetCourse()
        {
            var requestBody = Request.Body;
            Topic topic = await Request.ReadFromJsonAsync<Topic>()!;
            string fc = _taskManagerService.GetNewestValue(topic!.topic);
            FullCourse course = JsonConvert.DeserializeObject<FullCourse>(fc)!;
            return Ok(course);
            
        }



        [Route("api/courseList")]
        [HttpPost]
        public async Task<IActionResult> GetCourseList()
        {
            var requestBody = Request.Body;
            Topic topic = await Request.ReadFromJsonAsync<Topic>();
            string courseList = _taskManagerService.GetNewestValue(topic.topic);
            List<Course> courses = JsonConvert.DeserializeObject<List<Course>>(courseList);
            return Ok(courses);
            
           



            //return Ok(courses);
        }



        // GET: GetMqttValueController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GetMqttValueController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GetMqttValueController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GetMqttValueController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GetMqttValueController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GetMqttValueController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
