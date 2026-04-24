using StationDiagnosticService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationDiagnosticService.DAL
{
    internal interface IDiagnosticRepository
    {
        Task InsertDiagnosticData(DataFrame dataFrame);
    }
}
