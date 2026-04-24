using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationDiagnosticService.Services
{
    public  interface ITaskManagerService
    {
        Task ExecuteAsync();
    }
}
