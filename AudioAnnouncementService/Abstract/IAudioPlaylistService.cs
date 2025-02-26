using AudioAnnouncementService.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Abstract
{
    interface IAudioPlaylistService
    {
        Task<TrainAnnoucement> PrepareCoursePlaylist(FullCourse fullCourse);
        Task<List<AudioFileReader>> PrepareAnnoucementPLaylist(FullCourse delayedTrain);
    }
}
