using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ScheduleUpdater
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Schedule updater service is starting...");
            await WaitUntilStoppedAsync(stoppingToken);
        }

        private async Task WaitUntilStoppedAsync(CancellationToken stoppingToken)
        {

            HttpClient client = new HttpClient();
            /*client.DefaultRequestHeaders
            .Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));*/
            var values = new Dictionary<string, string>
            {
                { "msg", "Pociąg Intercity" },
                { "lang", "Ewa" },
                { "source", "ttsmp3" }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://ttsmp3.com/makemp3_new.php", content);

            var responseString = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseString);
            dynamic data = JObject.Parse(responseString);
            Console.WriteLine(data.URL);
            using (var download = new WebClient())
            {
                download.DownloadFile((string)data.URL, "..\\AudioAnnouncementService\\Sounds\\Core\\IC.mp3");
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);

            }

        }

    }
}
