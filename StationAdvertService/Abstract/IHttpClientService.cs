using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationAdvertService.Abstract
{
    internal interface IHttpClientService
    {
        Task<string> GetAdvertsAsync(string url);
    }
}
