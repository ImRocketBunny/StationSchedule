using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleUpdater.Services
{
    internal interface IFileReaderService
    {
        Task FileReaderManager(string file);
    }
}
