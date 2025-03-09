using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleUpdater.Models
{
    internal class ScheduleSource
    {
        public string? Name {  get; set; }
        public string? SourceLink { get; set; }
        public bool Update { get; set; }
    }
}
