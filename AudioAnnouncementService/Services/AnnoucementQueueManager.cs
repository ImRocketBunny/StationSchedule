using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Services
{
    internal class AnnoucementQueueManager : IAnnoucementQueueManager
    {
        private BlockingCollection<FullCourse> _annoucementTrainQueue;
        private BlockingCollection<FullCourse> _annoucementDelayQueue;

        public AnnoucementQueueManager()
        {
            _annoucementDelayQueue = new BlockingCollection<FullCourse>();
            _annoucementTrainQueue = new BlockingCollection<FullCourse>();
        }

        public void EnqueueTrainAnnoucement(FullCourse course)
          => _annoucementTrainQueue.Add(course);


        public void EnqueueDelayAnnoucement(FullCourse[] courses)
        {
            foreach (FullCourse fullCourse in courses)
            {
                if (!_annoucementDelayQueue.Contains(fullCourse))
                    _annoucementDelayQueue.Add(fullCourse);
            }
        }

        public IEnumerable<FullCourse> GetTrainAnnoucements()
            => _annoucementTrainQueue.GetConsumingEnumerable();

        public IEnumerable<FullCourse> GetDelayAnnoucements()
            => _annoucementDelayQueue.GetConsumingEnumerable();

    }
}
