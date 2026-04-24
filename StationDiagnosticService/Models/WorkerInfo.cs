using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationDiagnosticService.Models
{
    internal class WorkerInfo
    {
        public CancellationTokenSource Cts { get; init; }
        public int IsBusy;
    }
}
