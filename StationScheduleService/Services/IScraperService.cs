using StationScheduleService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal interface IScraperService
    {
        Task<List<Course>> GetScheduleContent(string HtmlContent);
    }
}
