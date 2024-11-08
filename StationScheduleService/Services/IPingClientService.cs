using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal interface IPingClientService
    {
        Task<bool> SendPing(string url);
    }
}
