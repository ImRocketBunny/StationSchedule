using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Models
{
    internal class Announcement
    {
        public string? Name {  get; set; }
        public string? FileName { get; set; }
        public DateTime? LastPlayed { get; set; }
    }
}
