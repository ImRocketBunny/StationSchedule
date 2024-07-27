using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    
    internal class HttpClientService : IHttpClientService
    {
        private readonly IConfiguration _configuration;
        private readonly ScrapingBrowser _browser;
        public HttpClientService(IConfiguration configuration)
        {
            _browser=new ScrapingBrowser
            {
                Timeout = TimeSpan.FromSeconds(int.Parse(configuration["ScraperOptions:ConnectionTimeout"]!))
            };
            _configuration = configuration;
            

        }
        public async Task<WebPage> ScrapAsync(string url)
        {
            return _browser.NavigateToPage(new Uri(url));
        }

        public Task<string> PrepareUrls()
        {
            //string url = "https://rozklad-pkp.pl/pl/sq?maxJourneys=&input=&REQStationS0F=excludeStationAttribute%3BM-&disableEquivs=yes&date=17.07.24&dateStart=17.07.24&REQ0JourneyDate=17.07.24&time=20%3A29&boardType=dep&GUIREQProduct_0=on&GUIREQProduct_1=on&GUIREQProduct_2=on&GUIREQProduct_3=on&maxJourneys=50&dateEnd=17.07.24&advancedProductMode=&start=#focus";
            string url = _configuration["ScraperOptions:Url"]!;
            url = url.Replace("date=", "date=");
            url = url.Replace("input=", "input=Warszawa+Zachodnia");
            url = url.Replace("selectDate=", "selectDate=today");
            url = url.Replace("dateBegin=", "dateBegin=" /*+ s*/);
            url = url.Replace("dateEnd=", "dateEnd=" /*+ s2*/);
            url = url.Replace("time=", "time=" + (DateTime.Now.TimeOfDay.ToString().Replace(":", "%3A")));
            url = url.Replace("maxJourneys=", "maxJourneys=20");
            url = url.Replace("boardType=", "boardType=dep");
            throw new NotImplementedException();
        }

      

        // public Task<string> PrepareUrl();
    }
}
