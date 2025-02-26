using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Models
{
    class DelayAnnoucement
    {
        public string? delay { get; set; }
        public AudioFileReader[]? playlist { get; set; }
        public DateTime lastPlayed { get; set; }
    }
}
