using AudioAnnouncementService.Models;
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
        IEnumerable<FullCourse> GetDelayAnnoucements();
        IEnumerable<FullCourse> GetTrainAnnoucements();
        void EnqueueDelayAnnoucement(FullCourse[] courses);
        void EnqueueTrainAnnoucement(FullCourse course);
    }
}
