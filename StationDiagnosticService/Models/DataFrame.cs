using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace StationDiagnosticService.Models
{
    internal class DataFrame
    {
        
        public string frameDate { get; set; }
        public string dataType { get; set; }

        public string dataSource { get; set; }

        public JsonObject jsonData { get; set; }

        public string textData { get; set; }



    }
}
