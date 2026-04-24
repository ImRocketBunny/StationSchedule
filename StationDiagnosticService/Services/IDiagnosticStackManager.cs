using StationDiagnosticService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace StationDiagnosticService.Services
{
    internal interface IDiagnosticStackManager
    {
        public void PrepareTextFrame(string topic, string textData, DateTime date);
        public void PrepareJsonFrame(string topic, JsonObject jsonData, DateTime date);
        public bool TryPopFrame(out DataFrame dataFrame);
        public int GetStackCount();
    }
}
