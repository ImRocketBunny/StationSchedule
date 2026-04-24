using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaConverter.Models
{
    internal class ConversionTask
    {
        private int id { get; set; }
        private string? name { get; set; }
        private string? description { get; set; }
        private decimal length { get; set; }
        private string? fileName { get; set; }
        private int status { get; set; }
    
    }
}
