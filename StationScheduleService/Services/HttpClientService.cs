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
            throw new NotImplementedException();
        }
    }
}
