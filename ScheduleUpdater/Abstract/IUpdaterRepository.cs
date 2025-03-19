using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleUpdater.Abstract
{
    interface IUpdaterRepository
    {
        Task UpdateSchedule(string content);
    }
}
