using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TextToSpeachUpdateService.Models;
namespace TextToSpeachUpdateService
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
            //string[] stacje = ["O�ar�w Mazowiecki", "Pruszk�w", "Warszawa �r�dmie�cie PKP", "Warszawa �r�dmie�cie PKP", "Pruszk�w", "Modlin", "Sulej�wek Mi�osna", "Skierniewice", "Skierniewice", "Otwock", "Gdynia G��wna", "Gdynia G��wna", "Pruszk�w", "Radzymin", "Piaseczno", "�yrard�w", "Celestyn�w", "Skierniewice", "Warszawa Wschodnia", "Pruszk�w", "Sulej�wek Mi�osna", "Warszawa Wschodnia", "Wroc�aw G��wny", "Otwock", "Berlin Hbf", "�owicz G��wny", "Siedlce", "Warszawa Lotnisko Chopina", "Modlin", "�yrard�w", "Pozna� G��wny", "Celestyn�w", "Grodzisk Mazowiecki PKP", "Mrozy", "Olsztyn G��wny", "Pruszk�w", "Sulej�wek Mi�osna", "Bia�ystok", "Otwock", "Warszawa Lotnisko Chopina", "Warszawa G��wna", "Warszawa Wschodnia", "Zakopane", "Warszawa Rembert�w", "Praha hl.n.", "Radzymin", "Mi�sk Mazowiecki", "Warszawa Lotnisko Chopina", "Skierniewice", "D�blin", "Gdynia G��wna"];
            var factory = new MqttFactory();

            // Create a MQTT client instance
            var mqttClient = factory.CreateMqttClient();

            // Create MQTT client options
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883) // MQTT broker address and port
                .WithCleanSession()
                .Build();


            var connectResult = await mqttClient.ConnectAsync(options);

            var msg = String.Empty;


            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                Console.WriteLine("Connected to MQTT broker successfully.");


                var response = await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/main/departures").Build());
                DirectoryInfo d3 = new DirectoryInfo("..\\AudioAnnouncementService\\Sounds\\Stations\\");
                FileInfo[] Files3 = d3.GetFiles("*.mp3");
                List<string> FileList3 = Files3.Select(e => e.Name.Replace(".mp3", "")).ToList();

                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    List<Course> courses = JsonConvert.DeserializeObject<List<Course>>(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
                    foreach (Course c in courses)
                    {
                        d3 = new DirectoryInfo("..\\AudioAnnouncementService\\Sounds\\Stations\\");
                        Files3 = d3.GetFiles("*.mp3");
                        FileList3 = Files3.Select(e => e.Name.Replace(".mp3", "")).ToList();
                        //string pattern = "[0-9][0-9]:[0-9][0-9]";
                        if (!FileList3.Contains(c.Headsign))
                        {
                            Console.WriteLine($"{c.Headsign}" + " missing");
                        }
                        d3 = new DirectoryInfo("..\\AudioAnnouncementService\\Sounds\\Stations\\");
                        Files3 = d3.GetFiles("*.mp3");
                        FileList3 = Files3.Select(e => e.Name.Replace(".mp3", "")).ToList();
                        var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(c.Route,"[0-9][0-9]:[0-9][0-9]",""), " �  ", " -  "),"  ","")," -");
                        foreach (string s in otherStations)
                        {
                           

                            if (!FileList3.Contains(s))
                            {
                                Console.WriteLine($"{s}" + "| missing");
                                HttpClient client = new HttpClient();
                                //client.DefaultRequestHeaders
                                //.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                                var values = new Dictionary<string, string>
                                {
                                    { "msg", s.ToString() },
                                    { "lang", "Ewa" },
                                    { "source", "ttsmp3" }
                                };

                                var content = new FormUrlEncodedContent(values);

                                var response = client.PostAsync("https://ttsmp3.com/makemp3_new.php", content);

                                var responseString = response.Result.Content.ReadAsStringAsync().Result;
                                //Console.WriteLine(responseString);
                                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                                //Console.WriteLine(data.URL);
                                using (var download = new WebClient())
                                {
                                   download.DownloadFile((string)data.URL, "..\\AudioAnnouncementService\\Sounds\\Stations\\" + s.ToString() + ".mp3");
                                }
                            }
                        }
                    }
                    
                    return Task.CompletedTask;
                };

            }
            /*for (int i= 10; i<60; i++) 
            {
                HttpClient client = new HttpClient();
                //client.DefaultRequestHeaders
                //.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var values = new Dictionary<string, string>
                {
                    { "msg", i.ToString() },
                    { "lang", "Ewa" },
                    { "source", "ttsmp3" }
                };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("https://ttsmp3.com/makemp3_new.php", content);

                var responseString = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(responseString);
                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                Console.WriteLine(data.URL);
                using (var download = new WebClient())
                {
                    download.DownloadFile((string)data.URL, "..\\AudioAnnouncementService\\Sounds\\Time\\Minutes\\" + i.ToString()+".mp3");
                }
            }
            /*HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders
            //.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var values = new Dictionary<string, string>
            {
                { "msg", "Poci�g Intercity" },
                { "lang", "Ewa" },
                { "source", "ttsmp3" }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://ttsmp3.com/makemp3_new.php", content);

            var responseString = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseString);
            dynamic data = Newtonsoft.Json.Linq.JObject.Parse(responseString);
            Console.WriteLine(data.URL);
            using (var download = new WebClient())
            {
                download.DownloadFile((string)data.URL, "..\\AudioAnnouncementService\\Sounds\\Core\\IC.mp3");
            }*/
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
