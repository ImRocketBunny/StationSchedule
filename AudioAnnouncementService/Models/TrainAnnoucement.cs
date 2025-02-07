using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Models
{
    internal class TrainAnnoucement
    {

        public List<AudioFileReader> playlist { get; set; }

    }
}