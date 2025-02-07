using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Models;
using NAudio.Wave.SampleProviders;
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
        private readonly IConfiguration _configuration;
        private readonly ILogger<AnnoucementQueueManager> _logger;
        private ConcurrentQueue<FullCourse> _annoucementTrainQueue;
        private ConcurrentQueue<FullCourse> _annoucementDelayQueue;
        private ConcurrentQueue<ConcatenatingSampleProvider> _annoucmentsToPlay;

        public AnnoucementQueueManager(IConfiguration configuration, ILogger<AnnoucementQueueManager> logger)
        {
            _configuration=configuration;
            _logger=logger;
            _annoucementDelayQueue = new ConcurrentQueue< FullCourse>();
            _annoucementTrainQueue = new ConcurrentQueue<FullCourse>();
            _annoucmentsToPlay = new ConcurrentQueue<ConcatenatingSampleProvider>();
        }

        public void EnqueueTrainAnnoucement(FullCourse course)
          => _annoucementTrainQueue.Enqueue(course);

        public void EnqueueReadyAnnoucement(ConcatenatingSampleProvider annoucement)
          => _annoucmentsToPlay.Enqueue(annoucement);

        public bool HasReadyAnnoucement()
        {
            return _annoucmentsToPlay.Any();
        }

        public void EnqueueDelayAnnoucement(FullCourse[] courses)
        {
            foreach (FullCourse fullCourse in courses)
            {
                if (!_annoucementDelayQueue.Contains(fullCourse))
                    _annoucementDelayQueue.Enqueue(fullCourse);
            }
        }

        public FullCourse GetTrainAnnoucements()
        {
            var res = _annoucementTrainQueue.TryDequeue(out var course);
            if (res)
            {
                return course!;
            }
            return null;
        }
           

        public FullCourse GetDelayAnnoucements()
        {
            var res = _annoucementDelayQueue.TryDequeue(out var delays);
            if (res)
            {
                return delays!;
            }
            return null;
        }

        public ConcatenatingSampleProvider GetReadyAnnoucement()
        {
            var res = _annoucmentsToPlay.TryDequeue(out var annoucement);
            if (res)
            {
                return annoucement!;
            }
            return null;
        }

    }
}
