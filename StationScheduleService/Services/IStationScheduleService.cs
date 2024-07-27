using StationScheduleService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal interface IStationScheduleService
    {
        Task<List<Course>> GetScheduleContent(string HtmlContent);
    }
}
