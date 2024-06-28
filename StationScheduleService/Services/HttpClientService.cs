using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal class HttpClientService : IHttpClientService
    {
        public Task<HttpResponseMessage> GetAsync(string url)
        {
            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage page = browser.NavigateToPage(new Uri(url));


            return null;

        }

        public Task<string> PrepareUrl()
        {
            throw new NotImplementedException();
        }

        // public Task<string> PrepareUrl();
    }
}
