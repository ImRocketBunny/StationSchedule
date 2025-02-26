using AudioAnnouncementService.Abstract;
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


        public List<AudioFileReader>? playlist { get; set; }
        public List<AudioFileReader>? introduction { get; set; }
        public List<AudioFileReader>? route { get; set; }
        public List<AudioFileReader>? details { get; set; }


    }
}