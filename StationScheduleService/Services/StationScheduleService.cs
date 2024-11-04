using HtmlAgilityPack;
using Newtonsoft.Json;
using ScrapySharp.Network;
using StationScheduleService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal sealed class StationScheduleService : IStationScheduleService
    {
        private readonly HtmlDocument _document;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StationScheduleService> _logger;
        private readonly IMqttManagerService _mqttManagerService;
        private readonly IWebScrapperService _httpClient;
        private readonly string _sessionId;
        private DateTime _nextScheduleSend;
        private Dictionary<string, List<Course>> _schedules;
        private List<string> _columns;
        private List<List<string>> _scheduleRows;
        private List<Course> _courses;
        private int _scheduleId = 0;
        private bool _connection = false;
        public StationScheduleService(IConfiguration configuration, ILogger<StationScheduleService> logger, IMqttManagerService mqttManagerService,
            IWebScrapperService httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _document = new HtmlDocument();
            _columns = new List<string>();
            _scheduleRows = new List<List<string>>();
            _courses = new List<Course>();
            _sessionId = $"{Guid.NewGuid()}-{DateTime.Now:yyyyMMdd}";
            _httpClient = httpClient;
            _mqttManagerService = mqttManagerService;
        }

        public async Task GetScheduleContent(string HtmlContent)
        {
            await GetScheduleData();
            if (_connection == false) return;
            await PrepareScheduleData(HtmlContent);
            await SendSchedule();

        }

        private async Task PrepareScheduleData(string HtmlContent)
        {
            _document.LoadHtml(HtmlContent);

            _columns = _document.DocumentNode.SelectSingleNode("//table[@class='hafasResult grey']")
                .Descendants("tr")
                .Where(tr => tr.Elements("th").Count() > 1)
                .Select(tr => tr.Elements("th")
                .Select(th => th.InnerText.Trim().Replace("\n", " ").Replace("&nbsp", ""))
                .ToList())
                .FirstOrDefault();

            _scheduleRows = _document.DocumentNode.SelectSingleNode("//table[@class='hafasResult grey']")
              .Descendants("tr")
              //.Skip(1)
              .Where(tr => tr.Elements("td").Count() > 1)
              .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim().Replace("\n", " ").Replace("&nbsp", ""))
              .ToList())
              .ToList();

            _scheduleRows.RemoveAt(_scheduleRows.Count - 1);


            foreach (var tableItem in _scheduleRows)
            {

                var c = new Course
                {
                    Time = tableItem[0],
                    Delay = _columns.Contains("Prognoza") ? tableItem[1] == "" || tableItem[1] == ";" ? tableItem[1].Replace(";", "") : tableItem[1].Split(";")[2] : "",
                    Name = _columns.Contains("Prognoza") ? System.Net.WebUtility.HtmlDecode(tableItem[2]) : System.Net.WebUtility.HtmlDecode(tableItem[1]),
                    Headsign = _columns.Contains("Prognoza") ? System.Net.WebUtility.HtmlDecode(tableItem[3].Split("     ")[0]) : System.Net.WebUtility.HtmlDecode(tableItem[2].Split("     ")[0]),
                    Route = _columns.Contains("Prognoza") ? System.Net.WebUtility.HtmlDecode(tableItem[3].Split("     ")[1]) : System.Net.WebUtility.HtmlDecode(tableItem[2].Split("     ")[1]),
                    Platform = _columns.Contains("Peron/stacja/przystanek") ? (_columns.Contains("Prognoza") ? tableItem[4].Replace(";", "").Trim() : tableItem[3].Replace(";", "").Trim()) : ""

                };
                _courses.Add(c);
               
            }
        }

        private async Task SendSchedule()
        {
            if (IsScheduleCompleted())
            {
                _logger.LogInformation("Sending schedule with ID: {ID}", _scheduleId);

        }

        private async Task GetScheduleData()
        {
            
            _connection=true;
        }

        private bool IsScheduleCompleted()
        {
            return _courses.Count > 0; 
        }

        Task<List<Course>> IStationScheduleService.GetScheduleContent(string HtmlContent)
        {
            throw new NotImplementedException();
        }
    }
}
