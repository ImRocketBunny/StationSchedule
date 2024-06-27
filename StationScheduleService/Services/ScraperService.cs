using StationScheduleService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal class ScraperService : IScraperService
    {
        public Task<List<Course>> GetScheduleContent(string HtmlContent)
        {
            throw new NotImplementedException();
        }
    }
}
