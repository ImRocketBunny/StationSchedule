using AudioAnnouncementService.Models;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Services
{
    internal interface IAudioService
    {
        Task PrepareCoursePlaylist(Course course);
        Task Play(ConcatenatingSampleProvider playlist);
        Task PrepareAnnoucementPLaylist(Announcement annoucement);
    }
}
