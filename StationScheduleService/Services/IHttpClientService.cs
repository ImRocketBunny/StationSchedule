using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal interface IHttpClientService
    {
        Task<HttpResponseMessage> GetAsync(string url);
        Task<string> PrepareUrl();
    }
}
