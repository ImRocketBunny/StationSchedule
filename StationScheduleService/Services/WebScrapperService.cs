using HtmlAgilityPack;
using Newtonsoft.Json;
using PuppeteerSharp;
using ScrapySharp.Network;
using StationScheduleService.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;



namespace StationScheduleService.Services
{
    
    internal class WebScrapperService : IWebScrapperService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StationScheduleService> _logger;
        //private IBrowser _browser;
        private IPage _page;
        //private ScrapingBrowser _browser;
        private List<Course> _arrivals;
        private List<Course> _departures;
        private HtmlDocument _documentArrivals;
        private HtmlDocument _documentDepartures;
        private bool _scrapCompleted;
        private bool _offlineDataFetch;

        public Dictionary<string, List<Course>> _schedule = new Dictionary<string, List<Course>>();

        public WebScrapperService(IConfiguration configuration, ILogger<StationScheduleService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            /*_browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                ExecutablePath = _configuration["ScraperOptions:ChromiumPath"]!, // Ścieżka do lokalnie zainstalowanego Chrome.
                Headless = true // Ustawienie trybu "bez głowy".
            });*/
            _configuration = configuration;
            _scrapCompleted = false;
            _arrivals = new List<Course>();
            _departures = new List<Course>();
            _documentArrivals = new HtmlDocument();
            _documentDepartures = new HtmlDocument();



        }
        public async Task<string> GetContentPage(string url)
        {
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                ExecutablePath = _configuration["ScraperOptions:ChromiumPath"]!,
                Headless = true
            });
            var page = await browser.NewPageAsync();
            try
            {
                Console.WriteLine(url);
                //var page = await browser.NewPageAsync();
                page.DefaultTimeout = 10000;
                await page.GoToAsync(url);
                return page.GetContentAsync().Result;
            }
            catch (Exception ex) 
            {
                _logger.LogCritical(ex.Message);
                return "";
            }
            finally
            {
                await page.CloseAsync();
                await browser.CloseAsync();
                
            }
            

           // return _page.GetContentAsync().Result;

        } 
            //=> await _browser.NavigateToPageAsync(new Uri(url));


      

        

        public string PrepareUrls(string type)
        {
            string url = _configuration["HTMLScrapConfiguration:HTMLLink"]!;
            DateTime dt = DateTime.Today;
            string s = dt.ToString("dd.MM.yy", CultureInfo.InvariantCulture);
            url = url.Replace("time=", "time=" + (DateTime.Now.TimeOfDay.ToString().Replace(":", "%3A")));
            url = url.Replace("dateStart=", "dateStart=" + s);
            url = url.Replace("&date=", "&date=" + s);
            url = url.Replace("JourneyDate=", "JourneyDate=" + s);
            url = url.Replace("boardType=", "boardType="+type);
            url = url.Replace("maxJourneys=", "maxJourneys="+ _configuration["StationConfiguration:MaxJourneys"]!);
            url = url.Replace("input=", "input="+_configuration["StationConfiguration:Name"]!.Replace(" ","+")+ "%235100067");
            return url;
        }

        public async Task<Dictionary<string, List<Course>>> ScrapPage()
        {   
            _scrapCompleted = false;
            _offlineDataFetch = false;
            ClearScrappedData();
            try
            {
                await Task.WhenAll(PrepareArrivals(), PrepareDepartures());
                ClearSchedule();
                _schedule.Add("arrivals", _arrivals!);
                _schedule.Add("departures", _departures!);
                _scrapCompleted = true;
                return _schedule;

            }
            catch (Exception ex)
            {
                _logger.LogError("Scrapping failed: "+ex.Message+"\nPrvious known data has will be fetchd");
               
                //_schedule.Add("arrivals", _arrivals!);
                //_schedule.Add("departures", _departures!);
                _scrapCompleted = true;
                _offlineDataFetch = true;
                return _schedule;
            }
            
            
        }

        private HtmlDocument PrepareHtml(string webContent)
        {
            var document = new HtmlDocument();
            document.LoadHtml(webContent);
            return document;
        }
          
        
           
        

        async Task PrepareArrivals()
        {
            _documentArrivals = PrepareHtml(await GetContentPage(PrepareUrls("arr")));
            List<string> columns = _documentArrivals.DocumentNode!.SelectSingleNode("//*[@id='wyniki']")!
                   .Descendants("tr")
                   .Where(tr => tr.Elements("th").Count() > 1)
                   .Select(tr => tr.Elements("th").Select(th => th.InnerText.Trim().Replace("\n", " ").Replace("&nbsp", "").Replace("NAZWA:", " ").Replace("zwiń:", "").Replace("rozwiń", "").Replace(";", "")).ToList()).FirstOrDefault();
            List<string> links = _documentArrivals.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
                .Descendants("a")
                .Where(a => a.Attributes["href"].Value.Contains("trainlink"))
                .Select(x => x.Attributes["href"].Value)
                .ToList();

           
            List<List<string>> table = _documentArrivals.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
            .Descendants("tr")
            .Skip(1)
            .Where(tr => tr.Elements("td").Count() > 1)
            .Select(td => td.Elements("td").Select(td => td.InnerText.Trim().Replace("\n", " ").Replace("&nbsp;", "").Replace("Nazwa:", " ")).ToList())
            .ToList();

            
            foreach (var tableItem in table)
            {
                Console.WriteLine("Adding arrival " + tableItem[1]);
                try
                {
                    var c = new Course
                    {
                        Time = HttpUtility.HtmlDecode(tableItem[columns!.IndexOf("Czas")]).Split("  ")[0],
                        Delay = columns.Contains("Prognoza") ? HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Prognoza")]).Replace("ok.+", "").Replace("min.", "").Replace("&#3210", "").Replace("&#3240", "").Trim() : string.Empty,
                        Name = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Kurs")]).Replace("ZOBACZ PEŁNĄ TRASĘ", string.Empty),
                        Headsign = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Z kierunku")]).Split("     ")[0],
                        Route = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Z kierunku")]).Split("     ")[1],
                        Platform = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Peron/tor")]).Replace(";", string.Empty).Trim(),
                        //Details = HttpUtility.HtmlDecode(links[table.IndexOf(tableItem)])
                    };
                    Console.WriteLine("Adding arrival " + c.Time);
                    _arrivals.Add(c);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(tableItem);
                }

            }

           
        }

        async Task PrepareDepartures()
        {
            _documentDepartures = PrepareHtml(await GetContentPage(PrepareUrls("dep")));
            //if (_documentDepartures.) {
            List<string> columns = _documentDepartures.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
                       .Descendants("tr")
                       .Where(tr => tr.Elements("th").Count() > 1)
                       .Select(tr => tr.Elements("th").Select(th => th.InnerText.Trim().Replace("\n", " ").Replace("&nbsp", "").Replace("NAZWA:", " ").Replace("zwiń:", "").Replace("rozwiń", "").Replace(";", "")).ToList()).FirstOrDefault();
                List<string> links = _documentDepartures.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
                    .Descendants("a")
                    .Where(a => a.Attributes["href"].Value.Contains("trainlink"))
                    .Select(x => x.Attributes["href"].Value)
                    .ToList();


                List<List<string>> table = _documentDepartures.DocumentNode.SelectSingleNode("//*[@id='wyniki']")
                .Descendants("tr")
                .Skip(1)
                .Where(tr => tr.Elements("td").Count() > 1)
                .Select(td => td.Elements("td").Select(td => td.InnerText.Trim().Replace("\n", " ").Replace("&nbsp;", "").Replace("Nazwa:", " ")).ToList())
                .ToList();

                
                foreach (var tableItem in table)
                {
                
                    var c = new Course
                    {
                        Time = HttpUtility.HtmlDecode(tableItem[columns!.IndexOf("Czas")]).Split("  ")[0],
                        Delay = columns.Contains("Prognoza") ? HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Prognoza")]).Replace("ok.+", "").Replace("min.", "").Replace("&#3210", "").Replace("&#3240", "").Trim() : string.Empty,
                        Name = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Kurs")]).Replace("ZOBACZ PEŁNĄ TRASĘ", string.Empty),
                        Headsign = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("W kierunku")]).Split("     ")[0],
                        Route = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("W kierunku")]).Split("     ")[1],
                        Platform = HttpUtility.HtmlDecode(tableItem[columns.IndexOf("Peron/tor")]).Replace(";", string.Empty).Trim(),
                        //Details = HttpUtility.HtmlDecode(links[table.IndexOf(tableItem)])
                    };
                    _departures.Add(c);
                }
        }

        public bool GetScrapperState()
        {
            return _scrapCompleted;
        }
        public bool OfflineData()
        {
            return _offlineDataFetch;
        }

        private void ClearScrappedData()
        {
            _arrivals.Clear();
            _departures.Clear();
        }

        private void ClearSchedule()
        {
            _schedule.Clear();
        }


        // public Task<string> PrepareUrl();
    }
}
