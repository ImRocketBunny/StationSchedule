using HtmlAgilityPack;
using Newtonsoft.Json;
using ScrapySharp.Network;
using StationScheduleService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal sealed class StationScheduleService : IStationScheduleService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StationScheduleService> _logger;
        private readonly IMqttManagerService _mqttManagerService;
        private readonly IWebScrapperService _webScrapperService;
        private readonly string _sessionId;
        private readonly IPingClientService _pingClientService;
        private DateTime _nextScheduleSend;
        private List<string> _list;
        private Dictionary<string, string> _schedules;
        private List<List<string>> _scheduleRows;
        private Dictionary<string, List<Course>> _courses;
        private Dictionary<string, List<Course>> _coursesHistory = new Dictionary<string, List<Course>>();
        private int _scheduleId = 0;
        private bool _connection = false;
        private bool _schedulePrepared = false;
        public StationScheduleService(IConfiguration configuration, ILogger<StationScheduleService> logger, IMqttManagerService mqttManagerService,
            IWebScrapperService webScrapperService, IPingClientService pingClientService)
        {
            _logger = logger;
            _configuration = configuration;
            _sessionId = $"{Guid.NewGuid()}-{DateTime.Now:yyyyMMdd}";
            _webScrapperService = webScrapperService;
            _mqttManagerService = mqttManagerService;
            _pingClientService = pingClientService;
            _schedules = new Dictionary<string, string>();
            _scheduleRows = new List<List<string>>();
            _courses = new Dictionary<string, List<Course>>();
            //_coursesHistory = new Dictionary<string, List<Course>>();
            _list = _configuration.GetSection("StationConfiguration:StationStructure").Get<List<string>>();

        }

        public async Task GetScheduleContent()
        {
            _connection = await _pingClientService.SendPing(_configuration["HTMLScrapConfiguration:HTMLPing"]!);

            if (_connection)
            {
                await GetScheduleData();
                await PrepareCourses();
                if (IsScheduleCompleted())
                    await SendSchedule();
            }

            
           

        }

        

        private async Task SendSchedule()
        {
            if (IsScheduleCompleted())
            {
                _logger.LogInformation("Sending schedule with ID: {ID}", _scheduleId);
                await _mqttManagerService.PublishSchedule(_schedules!);
                _courses.Clear();
                _schedules.Clear();
                _scheduleId++;
            }
        }

        private async Task GetScheduleData()
        {
            Console.WriteLine("History "+_coursesHistory.Count);

            _courses.Clear();
            _logger.LogInformation("Scrapping...");
            _courses = await _webScrapperService.ScrapPage();
            Console.WriteLine("History "+_coursesHistory.Count);
            Console.WriteLine("Current "+_courses.Count);
            if (_courses.Count!=2)
            {
                
                _courses.Clear();
                _logger.LogInformation("Fetching with saved Data");
                _courses.Add("departures", new List<Course>(_coursesHistory["departures"]));
                _courses.Add("arrivals", new List<Course>(_coursesHistory["arrivals"]));
            }
            else if(_courses.Count == 2)
            {
                if (_coursesHistory.Count == 0)
                {
                    _coursesHistory.Add("arrivals", new List<Course>(_courses["arrivals"]));
                    _coursesHistory.Add("departures", new List<Course>(_courses["departures"]));
                }
                else
                {
                    _coursesHistory["arrivals"] = new List<Course>(_courses["arrivals"]);
                    _coursesHistory["departures"] = new List<Course>(_courses["departures"]);
                    
                }



               
            }
           
        }

        private Task PrepareCourses()
        {
            _schedulePrepared = false;
            List<string> uniqueCourses = _courses["arrivals"].Select(course => course.Name).ToList().Concat(_courses["departures"].Select(course => course.Name).ToList()).Distinct().ToList();
            Dictionary<string, Course> arrivals = new Dictionary<string, Course>();
            _schedules.Clear();
            Dictionary<string, Course> departures = new Dictionary<string, Course>();
            _schedules.Add("main/departures", JsonConvert.SerializeObject(_courses["departures"], Formatting.Indented));
            _schedules.Add("main/arrivals", JsonConvert.SerializeObject(_courses["arrivals"], Formatting.Indented));
            //Console.WriteLine(JsonConvert.SerializeObject(_courses["arrivals"], Formatting.Indented));
            //Console.WriteLine("Przyjazdy " + _courses["arrivals"].Count);
            //Console.WriteLine("Odjazdy " + _courses["departures"].Count);
            arrivals.Clear();
            departures.Clear();
            foreach (var c in _courses["arrivals"])
            {
                if (!arrivals.ContainsKey(c.Name))
                {
                    arrivals.Add(c.Name, c);
                }
            }
            foreach (var c in _courses["departures"])
            {
                if (!departures.ContainsKey(c.Name))
                {
                    departures.Add(c.Name, c);
                }
            }


            List<FullCourse> fullCourses = new List<FullCourse>();
            fullCourses.Clear();
            foreach (var course in uniqueCourses)
            {

                var fc = new FullCourse
                {

                    Name = course,
                    ArrivalTime = arrivals.ContainsKey(course) ? arrivals[course].Time : null,
                    DepartureTime = departures.ContainsKey(course) ? departures[course].Time : null,
                    Delay = departures.ContainsKey(course) ? departures[course].Delay : arrivals[course].Delay,
                    HeadsignFrom = arrivals.ContainsKey(course) ? arrivals[course].Headsign : "",
                    HeadsignTo = departures.ContainsKey(course) ? departures[course].Headsign : "",
                    RouteFrom = arrivals.ContainsKey(course) ? arrivals[course].Route : "",
                    RouteTo = departures.ContainsKey(course) ? departures[course].Route : "",
                    Platform = (arrivals.ContainsKey(course) ? arrivals[course].Platform : (departures.ContainsKey(course) ? departures[course].Platform : ""))
                };
                fullCourses.Add(fc);
                
            }
            _schedules.Add("main/delayed", (JsonConvert.SerializeObject(fullCourses.Where(e => e.Delay != ""), Formatting.Indented)));
            foreach (string s3 in _list)
            {

                _schedules.Add(s3 + "/lcd", (JsonConvert.SerializeObject(fullCourses.Where(e => e.Platform.Contains(s3) && (int)(TimeOnly.Parse(e.DepartureTime ?? e.ArrivalTime).AddMinutes(e.Delay == "" ? 0.0 : Convert.ToDouble(e.Delay)) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes < 10 && (int)(TimeOnly.Parse(e.DepartureTime ?? e.ArrivalTime).AddMinutes(e.Delay == "" ? 0.0 : Convert.ToDouble(e.Delay)) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes >= -1).OrderBy(e => e.ArrivalTime ?? e.DepartureTime).FirstOrDefault(), Formatting.Indented)));
                _schedules.Add(s3 + "/audio", (JsonConvert.SerializeObject(fullCourses.Where(e => e.Platform.Contains(s3) && (int)(TimeOnly.Parse(e.DepartureTime ?? e.ArrivalTime).AddMinutes(e.Delay == "" ? 0.0 : Convert.ToDouble(e.Delay)) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes < 5 && (int)(TimeOnly.Parse(e.DepartureTime ?? e.ArrivalTime).AddMinutes(e.Delay == "" ? 0.0 : Convert.ToDouble(e.Delay)) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes >= -1).OrderBy(e => e.ArrivalTime ?? e.DepartureTime).FirstOrDefault(), Formatting.Indented)));

            }
            _courses.Clear();
            _schedulePrepared = true;
            return Task.CompletedTask; 
        }

        private bool IsScheduleCompleted()
        {
            return _webScrapperService.GetScrapperState() && _schedulePrepared;
        }

        /*Task<List<Course>> IStationScheduleService.GetScheduleContent(string HtmlContent)
        {
            throw new NotImplementedException();
        }*/
    }
}
