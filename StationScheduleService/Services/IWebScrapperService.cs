using ScrapySharp.Network;
using StationScheduleService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal interface IWebScrapperService
    {


        bool GetScrapperState();
        Task<Dictionary<string, List<Course>>> ScrapPage();

    }
}
