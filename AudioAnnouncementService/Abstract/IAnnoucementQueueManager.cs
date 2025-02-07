using AudioAnnouncementService.Models;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Abstract
{
    internal interface IAnnoucementQueueManager
    {
        FullCourse GetDelayAnnoucements();
        FullCourse GetTrainAnnoucements();
        void EnqueueDelayAnnoucement(FullCourse[] courses);
        void EnqueueTrainAnnoucement(FullCourse course);
        void EnqueueReadyAnnoucement(ConcatenatingSampleProvider annoucement);
        ConcatenatingSampleProvider GetReadyAnnoucement();
        bool HasReadyAnnoucement();


    }
}
