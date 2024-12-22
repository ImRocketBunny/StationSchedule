using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Models;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Services
{
    internal class AudioService : IAudioService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AudioService> _logger;
        private readonly IMqttManagerService _mqttManagerService;
        private readonly IAnnoucementQueueManager _annoucementQueueManager;
        private List<FullCourse> _trainToAnnouce;
        private List<FullCourse> _delayToAnnouce;
        private WasapiOut _audioVave;

        //private BlockingCollection<string> AudioToAnnouceQueue = new BlockingCollection<string>();
        public AudioService(IConfiguration configuration, ILogger<AudioService> logger, IMqttManagerService mqttManager,IAnnoucementQueueManager annoucementQueueManager) 
        {
            _configuration = configuration;
            _logger = logger;
            _mqttManagerService = mqttManager;
            _annoucementQueueManager = annoucementQueueManager;
            _audioVave = new WasapiOut();
        }

        public async Task Play(ConcatenatingSampleProvider playlist)
        {
            while (_audioVave.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(1000);
            }
            _audioVave.Init(playlist);
            _audioVave.Play();
            while (_audioVave.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(1000);
            }
        }

        public Task PrepareAnnoucementPLaylist(Announcement annoucement)
        {
            throw new NotImplementedException();
        }

        public Task PrepareCoursePlaylist(FullCourse course)
        {
            throw new NotImplementedException();
        }

        async Task SetUpAudioAnnoucement()
        {
            _trainToAnnouce = _annoucementQueueManager.GetTrainAnnoucements().ToList();
            _delayToAnnouce = _annoucementQueueManager.GetDelayAnnoucements().ToList();
            if (_trainToAnnouce.Count > 0)
            {

            }
            if(_delayToAnnouce.Count > 0)
            {

            }

        }

        static async Task RunPeriodicAnnoucement(TimeSpan interval, Func<Task> action)
        {
            while (true)
            {
                await action();
                await Task.Delay(interval);
            }
        }

        public Task PrepareCoursePlaylist(Course course)
        {
            throw new NotImplementedException();
        }
    }
}
