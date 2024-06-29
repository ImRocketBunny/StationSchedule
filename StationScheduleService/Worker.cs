using HtmlAgilityPack;
using ScrapySharp.Network;
using StationScheduleService.Models;
using Newtonsoft.Json;
using static Microsoft.FSharp.Core.ByRefKinds;
using System.Globalization;
using System.Web;
using static System.Net.Mime.MediaTypeNames;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace StationScheduleService
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
            DateTime dt = DateTime.Today;
            DateTime dt2= DateTime.Now.AddDays(6);
            string s = dt.ToString("dd.MM.yy", CultureInfo.InvariantCulture);
            string s2 = dt2.ToString("dd.MM.yy", CultureInfo.InvariantCulture);
            var url = "http://old.rozklad-pkp.pl/bin/stboard.exe/pn?ld=mobil&input=&REQStationS0F=excludeStationAttribute%3BM-&disableEquivs=yes&selectDate=&dateBegin=&dateEnd=&time=&timeselect=wybierz+z+listy&boardType=&advancedProductMode=&GUIREQProduct_0=on&GUIREQProduct_1=on&GUIREQProduct_2=on&GUIREQProduct_3=on&maxJourneys=&start=Poka%C5%BC";

            url = url.Replace("input=", "input=Warszawa+Zachodnia");
            url = url.Replace("selectDate=", "selectDate=today");
            url = url.Replace("dateBegin=", "dateBegin=" + s);
            url = url.Replace("dateEnd=", "dateEnd=" + s2);
            url = url.Replace("time=", "time=" + (DateTime.Now.TimeOfDay.ToString().Replace(":", "%3A")));
            url = url.Replace("maxJourneys=", "maxJourneys=20");
            url = url.Replace("boardType=", "boardType=dep");
            Console.WriteLine(url);
            //ScrapingBrowser Browser = new ScrapingBrowser();
            //WebPage PageResult = Browser.NavigateToPage(new Uri(url));
            //HtmlNode rawHTML = PageResult.Html;
            //Console.WriteLine(rawHTML.InnerHtml);

            ScrapingBrowser browser = new ScrapingBrowser();

            
            WebPage page = browser.NavigateToPage(new Uri(url));

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(page.Content);

            //var q = document.DocumentNode.SelectNodes("/ html / body / div[2] / div / div[2] / table / tbody / tr[2] / td / div[2] / div[3] / div / div / div / div / div / div / div / div / div / div / table[1]");


            List<string> columns = document.DocumentNode.SelectSingleNode("//table[@class='hafasResult grey']")
                .Descendants("tr")
                .Where(tr => tr.Elements("th").Count() > 1)
                .Select(tr => tr.Elements("th").Select(th => th.InnerText.Trim().Replace("\n", " ").Replace("&nbsp", "")).ToList()).FirstOrDefault();



            







            List<List<string>> table = document.DocumentNode.SelectSingleNode("//table[@class='hafasResult grey']")
              .Descendants("tr")
              .Skip(1)
              .Where(tr => tr.Elements("td").Count() > 1)
              .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim().Replace("\n", " ").Replace("&nbsp","")).ToList())
              .ToList();


            Console.WriteLine(columns.Contains("Prognoza"));

            /*foreach(List<string> str in table)
            {
                foreach(string str2 in str)
                {
                    Console.Write(str2);
                }
                Console.WriteLine();
                
            }*/

            //table.RemoveAt(0);
            table.RemoveAt(table.Count-1);
            List<Course> courses = new List<Course>();
            foreach (var tableItem in table) {
                
                var c = new Course
                {
                    Time = tableItem[0],
                    Delay = columns.Contains("Prognoza")?tableItem[1] == ""||tableItem[1]==";" ? tableItem[1].Replace(";","") :tableItem[1].Split(";")[2]:"",
                    Name = columns.Contains("Prognoza") ? System.Net.WebUtility.HtmlDecode(tableItem[2]): System.Net.WebUtility.HtmlDecode(tableItem[1]),
                    Headsign = columns.Contains("Prognoza") ? System.Net.WebUtility.HtmlDecode(tableItem[3].Split("     ")[0]): System.Net.WebUtility.HtmlDecode(tableItem[2].Split("     ")[0]),
                    Route = columns.Contains("Prognoza") ? System.Net.WebUtility.HtmlDecode(tableItem[3].Split("     ")[1]): System.Net.WebUtility.HtmlDecode(tableItem[2].Split("     ")[1]),
                    Platform = columns.Contains("Peron/stacja/przystanek")?(columns.Contains("Prognoza") ? tableItem[4].Replace(";","").Trim(): tableItem[3].Replace(";", "").Trim()):""

                };
                courses.Add(c);
             
            }
            Dictionary<string, string> openWith =new Dictionary<string, string>();
            var json = JsonConvert.SerializeObject(courses, Formatting.Indented);
            //HtmlDocument doc = new HtmlDocument();
            List<string> list = new List<string>();
            foreach(Course pl in courses.GroupBy(e=>e.Platform)
                   .Select(grp => grp.First())
                   .ToList())
            {
                list.Add(pl.Platform);
            }
            list.Sort();
            Console.WriteLine(json);
            openWith.Add("station/main/dep", json);
            foreach( string s3 in list )
            {
                openWith.Add(s3,(JsonConvert.SerializeObject(courses.Where(e => e.Platform == s3).FirstOrDefault(), Formatting.Indented)));
            }

            /*List<List<string>> table = doc.DocumentNode.SelectSingleNode("//table[@class='hafasResultGrey']")
            .Descendants("tr")
            .Skip(1)
            .Where(tr => tr.Elements("td").Count() > 1)
            .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
            .ToList();*/


            // html / body / div[2] / div / div[2] / table / tbody / tr[2] / td / div[2] / div[3] / div / div / div / div / div / div / div / div / div / div / table[1]
            /*int pFrom = page.Content.IndexOf("<table cellspacing=\"0\"");
            int pTo = page.Content.LastIndexOf("<table class=\"hafasButtons\" cellspacing=\"0\">");
            string result = page.Content.Substring(pFrom, pTo - pFrom);

            Console.WriteLine(result);*/

            //Perform XPath query

            //("table.hafasResult.grey")[0];
            //Console.WriteLine(nodes!=null);







            /*while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }*/
            
            var factory = new MqttFactory();

            // Create a MQTT client instance
            var mqttClient = factory.CreateMqttClient();

            // Create MQTT client options
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883) // MQTT broker address and port
                .WithCleanSession()
                .Build();


            var connectResult = await mqttClient.ConnectAsync(options);

            // Set up handlers

            // Connect to broker
            

            //var AppName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("StationConfiguration:StationStructure").Get<List<string>>();
            foreach (string s4 in openWith.Keys)
            {
                
                {
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(s4)
                        .WithPayload(openWith.GetValueOrDefault(s4))
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithRetainFlag()
                        .Build();

                    await mqttClient.PublishAsync(message);
                    
                }
            }
            
        }
    }
}
