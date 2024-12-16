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
    internal class AudioService : IAudioService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AudioService> _logger;
        private readonly IMqttManagerService _mqttManagerService;
        private BlockingCollection<string> AudioToAnnouceQueue = new BlockingCollection<string>();
        public AudioService(IConfiguration configuration, ILogger<AudioService> logger, IMqttManagerService mqttManager) 
        {
            _configuration = configuration;
            _logger = logger;
            _mqttManagerService = mqttManager;
        }

        public Task Play(ConcatenatingSampleProvider playlist)
        {
            throw new NotImplementedException();
        }

        public Task PrepareAnnoucementPLaylist(Announcement annoucement)
        {
            throw new NotImplementedException();
        }

        public Task PrepareCoursePlaylist(Course course)
        {
            throw new NotImplementedException();
        }

        async Task SetUpAudioAnnoucement()
        {
            await _mqttManagerService.ReceiveMqttDataAsync(AudioToAnnouceQueue);
            Task consumerTask = Task.Run(() =>
            {
                foreach (var msg in AudioToAnnouceQueue.GetConsumingEnumerable())
                {
                     // przygotuj i wygłoś
                }
            });

        }
    }
}
