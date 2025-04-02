using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.DAL.Abstract
{
    interface IStationRepository
    {
        Task<List<string>> GetStationStructure(int stationId);
    }
}
