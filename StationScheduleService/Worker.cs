using HtmlAgilityPack;
using ScrapySharp.Network;
using StationScheduleService.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Web;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Resources;
using System.Reflection.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Collections;


namespace StationScheduleService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private Dictionary<string, string> openWithOlds = new Dictionary<string, string>();

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration=configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await WaitUntilStoppedAsync(stoppingToken);
        }

        private  async Task WaitUntilStoppedAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime dt = DateTime.Today;
                DateTime dt2 = DateTime.Now.AddDays(6);
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
                //Console.WriteLine(url);
                //ScrapingBrowser Browser = new ScrapingBrowser();
                //WebPage PageResult = Browser.NavigateToPage(new Uri(url));
                //HtmlNode rawHTML = PageResult.Html;
                //Console.WriteLine(rawHTML.InnerHtml);

                ScrapingBrowser browser = new ScrapingBrowser();

                browser.Timeout = TimeSpan.FromSeconds(30);


                url = "https://rozklad-pkp.pl/pl/sq?maxJourneys=20&start=yes&dirInput=&GUIREQProduct_0=on&GUIREQProduct_1=on&GUIREQProduct_2=on&GUIREQProduct_3=on&advancedProductMode=&boardType=&input=&input=5100067&date=&dateStart=&REQ0JourneyDate=&time=";
                url = url.Replace("time=", "time=" + (DateTime.Now.TimeOfDay.ToString().Replace(":", "%3A")));
                url = url.Replace("dateStart=", "dateStart=" + s);
                url = url.Replace("&date=", "&date=" + s);
                url = url.Replace("JourneyDate=", "JourneyDate=" + s);
                url = url.Replace("boardType=", "boardType=dep");
                Console.WriteLine(url);

                var url2 = "https://rozklad-pkp.pl/pl/sq?maxJourneys=20&start=yes&dirInput=&GUIREQProduct_0=on&GUIREQProduct_1=on&GUIREQProduct_2=on&GUIREQProduct_3=on&advancedProductMode=&boardType=&input=&input=5100067&date=&dateStart=&REQ0JourneyDate=&time=";
                url2 = url2.Replace("time=", "time=" + (DateTime.Now.TimeOfDay.ToString().Replace(":", "%3A")));
                url2 = url2.Replace("dateStart=", "dateStart=" + s);
                url2 = url2.Replace("&date=", "&date=" + s);
                url2 = url2.Replace("JourneyDate=", "JourneyDate=" + s);
                url2 = url2.Replace("boardType=", "boardType=arr");
                Console.WriteLine(url2);
                WebPage page;
                WebPage page2;
                try
                {
                    //Console.WriteLine(url);
                    page = browser.NavigateToPage(new Uri(url));
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(page.Content);
                    page2 = browser.NavigateToPage(new Uri(url2));
                    HtmlDocument document2 = new HtmlDocument();
                    document2.LoadHtml(page2.Content);
                    List<Course> dep = await PrepareDepartures(document);
                    List<Course> arr = await PrepareArrivals(document2);
                    await PrepareSchedule(arr,dep);
                    Thread.Sleep(20000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message+" "+DateTime.Now);
                  
                }
                //*[@id="wyniki"]/tbody/tr[1]/td[4]/div/a
                //url = url.Replace("time=", "time=" + (DateTime.Now.TimeOfDay.ToString().Replace(":", "%3A")));

                //HtmlDocument document = new HtmlDocument();

                //document.LoadHtml(page.Content);

                //Console.WriteLine(HttpUtility.HtmlDecode(page.Content));
                //var q = document.DocumentNode.SelectNodes("/ html / body / div[2] / div / div[2] / table / tbody / tr[2] / td / div[2] / div[3] / div / div / div / div / div / div / div / div / div / div / table[1]");


            }

        }
        async Task PrepareSchedule(List<Course> arr,List<Course> dep)
        {
            /*List<string> columns = document.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
                   .Descendants("tr")
                   .Where(tr => tr.Elements("th").Count() > 1)
                   .Select(tr => tr.Elements("th").Select(th => th.InnerText.Trim().Replace("\n", " ").Replace("&nbsp", "").Replace("NAZWA:", " ").Replace("zwiń:", "").Replace("rozwiń", "").Replace(";", "")).ToList()).FirstOrDefault();
            List<string> links = document.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
                .Descendants("a")
                .Where(a => a.Attributes["href"].Value.Contains("trainlink"))
                .Select(x => x.Attributes["href"].Value)
                .ToList();

            foreach (string link in columns)
            {
                Console.WriteLine(HttpUtility.HtmlDecode(link));
            }


            /*List<string> columns = document.DocumentNode.SelectSingleNode("//table[@class='f-table table desktop breakpoint footable-loaded footable']")
            .Descendants("tr")
            .Where(tr => tr.Elements("th").Count() > 1)
            .Select(tr => tr.Elements("th").Select(th => th.InnerText.Trim().Replace("\n", " ").Replace("&nbsp", "")).ToList()).FirstOrDefault();*/

            //var content = document.DocumentNode.SelectSingleNode("//*[@id='wyniki']");
            //Console.WriteLine(content==null);

            /*List<List<string>> table = document.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
            .Descendants("tr")
            //.Skip(1)
            .Where(tr => tr.Elements("td").Count() > 1)
            .Select(td => td.Elements("td").Select(td => td.InnerText.Trim().Replace("\n", " ").Replace("&nbsp;", "").Replace("Nazwa:", " ")).ToList())
            .ToList();*/

            /*List<Course> courses = new List<Course>();
            foreach (var tableItem in table)
            {

                var c = new Course
                {
                    Time = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Czas")]).Split("  ")[0],
                    Delay = columns.Contains("Prognoza") ? HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Prognoza")]).Replace("ok.+", "").Replace("min.", "").Replace("&#3210", "").Replace("&#3240", "").Trim() : string.Empty,
                    Name = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Kurs")]).Replace("ZOBACZ PEŁNĄ TRASĘ", string.Empty),
                    Headsign = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("W kierunku")]).Split("     ")[0],
                    Route = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("W kierunku")]).Split("     ")[1],
                    Platform = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Peron/tor")]).Replace(";", string.Empty).Trim(),
                    Details = HttpUtility.HtmlDecode(links[table.IndexOf(tableItem)])
                };
                courses.Add(c);

            }
            var json = JsonConvert.SerializeObject(courses, Formatting.Indented);

            //TimeSpan timeDiff = TimeOnly.Parse(DateTime.Now.ToString("HH:mm")) - TimeOnly.Parse("14:31");
            //Console.WriteLine(string.Format("{0}",
            //                    (int)timeDiff.TotalMinutes));


            Console.WriteLine(json);





            /*List<List<string>> table = document.DocumentNode.SelectSingleNode("//table[@class='hafasResult grey']")
              .Descendants("tr")
              .Skip(1)
              .Where(tr => tr.Elements("td").Count() > 1)
              .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim().Replace("\n", " ").Replace("&nbsp","")).ToList())
              .ToList();/


            //Console.WriteLine(columns.Contains("Prognoza"));

            /*foreach(List<string> str in table)
            {
                foreach(string str2 in str)
                {
                    Console.Write(str2);
                }
                Console.WriteLine();

            }*/

            //table.RemoveAt(0);
            //table.RemoveAt(table.Count-1);
            //List<Course> courses = new List<Course>();
            /*foreach (var tableItem in table) {

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

            }*/
            Dictionary<string, string> openWith = new Dictionary<string, string>();
            //var json = JsonConvert.SerializeObject(courses, Formatting.Indented);
            //HtmlDocument doc = new HtmlDocument();
            List<string> list = _configuration.GetSection("StationConfiguration:StationStructure").Get<List<string>>();
            /*foreach (Course pl in courses.GroupBy(e=>e.Platform)
                   .Select(grp => grp.First())
                   .ToList())
            {
                list.Add(pl.Platform);
            }*/

            /*foreach (string s3 in list)
            {
                Console.WriteLine(s3);
                //openWith.Add(s3, (JsonConvert.SerializeObject(courses.Where(e => e.Platform.Contains(s3)).FirstOrDefault(), Formatting.Indented)));
            }*/

            list.Sort();
            //Console.WriteLine(json);
            openWith.Add("main/departures", JsonConvert.SerializeObject(dep, Formatting.Indented));
            openWith.Add("main/arrivals", JsonConvert.SerializeObject(arr, Formatting.Indented));


            List<string> uniqueCourses = arr.Select(course => course.Name).ToList().Concat(dep.Select(course => course.Name).ToList()).Distinct().ToList();
            Dictionary<string, Course> arrivals = new Dictionary<string, Course>();
            Dictionary<string, Course> departures = new Dictionary<string, Course>();
            foreach (var c in arr)
            {
                if (!arrivals.ContainsKey(c.Name))
                {
                    arrivals.Add(c.Name, c);
                } 
            }
            foreach (var c in dep)
            {
                if (!departures.ContainsKey(c.Name))
                {
                    departures.Add(c.Name, c);
                }
            }


            List<FullCourse> fullCourses = new List<FullCourse>();
            foreach (var course in uniqueCourses)
            {

                var fc = new FullCourse
                {

                    Name = course,
                    ArrivalTime = arrivals.ContainsKey(course)?arrivals[course].Time : null,
                    DepartureTime = departures.ContainsKey(course) ? departures[course].Time : null,
                    Delay = departures.ContainsKey(course) ? departures[course].Delay : arrivals[course].Delay,
                    HeadsignFrom = arrivals.ContainsKey(course) ? arrivals[course].Headsign: "",
                    HeadsignTo = departures.ContainsKey(course) ? departures[course].Headsign : "",
                    RouteFrom = arrivals.ContainsKey(course) ? arrivals[course].Route : "",
                    RouteTo = departures.ContainsKey(course) ? departures[course].Route : "",
                    Platform = (arrivals.ContainsKey(course) ? arrivals[course].Platform : (departures.ContainsKey(course) ? departures[course].Platform:""))
                };
                fullCourses.Add(fc);

            }
            //uniqueCourses.AddRange(dep.Select(course => course.Name.Where());
            //uniqueCourses.AddRange(dep.Select(course => course.Name.Where(!(arr.Select(course => course.Name).ToList()).Contains(course.Name)))); 
            //(dep.Select(course => arr.All(c2 => c2.Name != course.Name));



            openWith.Add("main/delayed", (JsonConvert.SerializeObject(fullCourses.Where(e=>e.Delay!=""), Formatting.Indented)));

            foreach (string s3 in list)
            {
                
                openWith.Add(s3+"/lcd", (JsonConvert.SerializeObject(fullCourses.Where(e => e.Platform.Contains(s3) && (int)(TimeOnly.Parse(e.DepartureTime??e.ArrivalTime).AddMinutes(e.Delay == "" ? 0.0 : Convert.ToDouble(e.Delay)) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes < 10).OrderBy(e=>e.ArrivalTime??e.DepartureTime).FirstOrDefault(), Formatting.Indented)));
                openWith.Add(s3+"/audio", (JsonConvert.SerializeObject(fullCourses.Where(e => e.Platform.Contains(s3) && (int)(TimeOnly.Parse(e.DepartureTime ?? e.ArrivalTime).AddMinutes(e.Delay == "" ? 0.0 : Convert.ToDouble(e.Delay)) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes < 4).OrderBy(e => e.ArrivalTime ?? e.DepartureTime).FirstOrDefault(), Formatting.Indented)));
                    
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
                        .WithTopic("station/" + s4)
                        .WithPayload(openWith.GetValueOrDefault(s4) == "null" ? "{}" : openWith.GetValueOrDefault(s4))
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithRetainFlag()
                        .Build();
                    if ((openWithOlds.ContainsKey(s4)&& openWithOlds[s4]!=openWith[s4])||openWithOlds.Keys.Count==0)
                    await mqttClient.PublishAsync(message);

                }
            }
            openWithOlds = openWith;
        }

        async Task<List<Course>> PrepareArrivals(HtmlDocument document)
        {
            List<string> columns = document.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
                   .Descendants("tr")
                   .Where(tr => tr.Elements("th").Count() > 1)
                   .Select(tr => tr.Elements("th").Select(th => th.InnerText.Trim().Replace("\n", " ").Replace("&nbsp", "").Replace("NAZWA:", " ").Replace("zwiń:", "").Replace("rozwiń", "").Replace(";", "")).ToList()).FirstOrDefault();
            List<string> links = document.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
                .Descendants("a")
                .Where(a => a.Attributes["href"].Value.Contains("trainlink"))
                .Select(x => x.Attributes["href"].Value)
                .ToList();

            /*foreach (string link in columns)
            {
                Console.WriteLine(HttpUtility.HtmlDecode(link));
            }*/


            /*List<string> columns = document.DocumentNode.SelectSingleNode("//table[@class='f-table table desktop breakpoint footable-loaded footable']")
            .Descendants("tr")
            .Where(tr => tr.Elements("th").Count() > 1)
            .Select(tr => tr.Elements("th").Select(th => th.InnerText.Trim().Replace("\n", " ").Replace("&nbsp", "")).ToList()).FirstOrDefault();*/

            /*var content = document.DocumentNode.SelectSingleNode("//*[@id='wyniki']");
            Console.WriteLine(content==null);*/

            List<List<string>> table = document.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
            .Descendants("tr")
            //.Skip(1)
            .Where(tr => tr.Elements("td").Count() > 1)
            .Select(td => td.Elements("td").Select(td => td.InnerText.Trim().Replace("\n", " ").Replace("&nbsp;", "").Replace("Nazwa:", " ")).ToList())
            .ToList();

            List<Course> courses = new List<Course>();
            foreach (var tableItem in table)
            {

                var c = new Course
                {
                    Time = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Czas")]).Split("  ")[0],
                    Delay = columns.Contains("Prognoza") ? HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Prognoza")]).Replace("ok.+", "").Replace("min.", "").Replace("&#3210", "").Replace("&#3240", "").Trim() : string.Empty,
                    Name = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Kurs")]).Replace("ZOBACZ PEŁNĄ TRASĘ", string.Empty),
                    Headsign = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Z kierunku")]).Split("     ")[0],
                    Route = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Z kierunku")]).Split("     ")[1],
                    Platform = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Peron/tor")]).Replace(";", string.Empty).Trim(),
                    //Details = HttpUtility.HtmlDecode(links[table.IndexOf(tableItem)])
                };
                courses.Add(c);

            }
            
           return courses;
        }

        async Task<List<Course>> PrepareDepartures(HtmlDocument document)
        {
            List<string> columns = document.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
                   .Descendants("tr")
                   .Where(tr => tr.Elements("th").Count() > 1)
                   .Select(tr => tr.Elements("th").Select(th => th.InnerText.Trim().Replace("\n", " ").Replace("&nbsp", "").Replace("NAZWA:", " ").Replace("zwiń:", "").Replace("rozwiń", "").Replace(";", "")).ToList()).FirstOrDefault();
            List<string> links = document.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
                .Descendants("a")
                .Where(a => a.Attributes["href"].Value.Contains("trainlink"))
                .Select(x => x.Attributes["href"].Value)
                .ToList();

            /*foreach (string link in columns)
            {
                Console.WriteLine(HttpUtility.HtmlDecode(link));
            }*/


            /*List<string> columns = document.DocumentNode.SelectSingleNode("//table[@class='f-table table desktop breakpoint footable-loaded footable']")
            .Descendants("tr")
            .Where(tr => tr.Elements("th").Count() > 1)
            .Select(tr => tr.Elements("th").Select(th => th.InnerText.Trim().Replace("\n", " ").Replace("&nbsp", "")).ToList()).FirstOrDefault();*/

            /*var content = document.DocumentNode.SelectSingleNode("//*[@id='wyniki']");
            Console.WriteLine(content==null);*/

            List<List<string>> table = document.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
            .Descendants("tr")
            //.Skip(1)
            .Where(tr => tr.Elements("td").Count() > 1)
            .Select(td => td.Elements("td").Select(td => td.InnerText.Trim().Replace("\n", " ").Replace("&nbsp;", "").Replace("Nazwa:", " ")).ToList())
            .ToList();

            List<Course> courses = new List<Course>();
            foreach (var tableItem in table)
            {

                var c = new Course
                {
                    Time = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Czas")]).Split("  ")[0],
                    Delay = columns.Contains("Prognoza") ? HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Prognoza")]).Replace("ok.+", "").Replace("min.", "").Replace("&#3210", "").Replace("&#3240", "").Trim() : string.Empty,
                    Name = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Kurs")]).Replace("ZOBACZ PEŁNĄ TRASĘ", string.Empty),
                    Headsign = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("W kierunku")]).Split("     ")[0],
                    Route = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("W kierunku")]).Split("     ")[1],
                    Platform = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Peron/tor")]).Replace(";", string.Empty).Trim(),
                    //Details = HttpUtility.HtmlDecode(links[table.IndexOf(tableItem)])
                };
                courses.Add(c);

            }

            return courses;
        }

    }
    
}
