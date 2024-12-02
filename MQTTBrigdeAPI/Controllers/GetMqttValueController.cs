using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MQTTBrigdeAPI.Models;
using MQTTBrigdeAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace MQTTBrigdeAPI.Controllers
{
    public class GetMqttValueController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private ITaskManagerService _taskManagerService;
        public GetMqttValueController(ILogger logger, IConfiguration configuration,ITaskManagerService taskManagerService) 
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

        // GET: GetMqttValueController/Details
        [Route("api/Details")]
        [HttpPost]
        public async Task<IActionResult> GetByTopic()
        {
            var requestBody = Request.Body;
            Topic topic = await Request.ReadFromJsonAsync<Topic>();
            string fc = _taskManagerService.GetNewestValue(topic.topic);
            FullCourse courses = JsonConvert.DeserializeObject<FullCourse>(fc);
            

            return Ok(courses);
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
