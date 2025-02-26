using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Models;
using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Collections.Concurrent;


namespace AudioAnnouncementService.Services
{
    internal class AudioService : IAudioService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AudioService> _logger;
        private readonly IMqttManagerService _mqttManagerService;
        private readonly IAnnoucementQueueManager _annoucementQueueManager;
        private readonly IAudioFileService _audioFileService;
        private readonly IAudioPlaylistService _audioPlaylistService;
        private FullCourse _trainToAnnouce;
        private FullCourse _delayToAnnouce;
        private ConcurrentDictionary<string, DelayAnnoucement> _currentDelays = new ConcurrentDictionary<string, DelayAnnoucement>();

        public AudioService(IConfiguration configuration, 
            ILogger<AudioService> logger, 
            IMqttManagerService mqttManager,
            IAnnoucementQueueManager annoucementQueueManager, 
            IAudioFileService audioFileService,
            IAudioPlaylistService audioPlaylistService) 
        {
            _configuration = configuration;
            _logger = logger;
            _mqttManagerService = mqttManager;
            _annoucementQueueManager = annoucementQueueManager;
            _audioFileService = audioFileService;
            _audioPlaylistService = audioPlaylistService;
            
        }

        public async Task RunAudioService()
        {
            await SetUpTrainAudioAnnoucementAsync();
            await SetUpDelayAudioAnnoucementAsync();
            //Setup Any Anooucemment
            await Play();
        }
        private async Task Play()
        {
            if (_annoucementQueueManager.HasReadyAnnoucement())
            {

                var audioVave= new WasapiOut();
                var playlist = _annoucementQueueManager.GetReadyAnnoucement();
                audioVave.Init(playlist);
                audioVave.Play();
                _logger.LogInformation($"Playing train annoucement...");
                while (audioVave.PlaybackState == PlaybackState.Playing)
                {
                    await Task.Delay(50);
                    //_logger.LogInformation($"Progress: {audioVave.GetPositionTimeSpan()}");
                }
                audioVave.Stop();
                audioVave.Dispose();
                _logger.LogInformation($"Finnished playing train annoucement");

            }else if (_currentDelays.Count > 0)
            {
                if (_currentDelays.Any(e => _currentDelays[e.Key].lastPlayed < DateTime.Now))
                {
                    foreach (var key in _currentDelays.Keys)
                    {
                        if (_currentDelays[key].lastPlayed < DateTime.Now)
                        {
                            var audioVave = new WasapiOut();
                            var playlist = _currentDelays[key].playlist;
                            _logger.LogInformation($"Playing delay annoucement...");

                            foreach (var file in playlist!)
                            {
                                using (var audioFile = file)
                                using (var outputDevice = new WaveOutEvent())
                                {
                                    outputDevice.Init(audioFile);
                                    outputDevice.Play();
                                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                                    {
                                        await Task.Delay(16);
                                    }
                                    outputDevice.Stop();
                                    outputDevice.Dispose();
                                }
                                _currentDelays[key].lastPlayed = DateTime.Now.AddMinutes(7);
                            }
                            _logger.LogInformation($"Finnished playing delay annoucement");
                            break;
                        }
                    }
                }
            }else if (true)
            {

            }
            _logger.LogInformation($"There are {_currentDelays.Keys.Count} active delay annoucements");
            _logger.LogInformation($"There are {_annoucementQueueManager.QueuedAnnoucements()} active train annoucements");

        }


        private async Task HandleDelayAnnoucement(FullCourse delayedTrain)
        {
            await Task.Run(async () =>
            {
                if (!_currentDelays.ContainsKey(delayedTrain.Name!) || _currentDelays[delayedTrain.Name!].delay != delayedTrain.Delay)
                {
                    var trainAnnoucement = await Task.Run(() => PrepareAnnoucementPLaylist(delayedTrain));

                    if (trainAnnoucement.playlist!.ToArray().Length > 0)
                        if (!_currentDelays.ContainsKey(delayedTrain.Name!))
                            _currentDelays.TryAdd(delayedTrain.Name!, new DelayAnnoucement { delay = delayedTrain.Delay, playlist = trainAnnoucement.playlist.ToArray(),lastPlayed = DateTime.Now});
                        else
                            _currentDelays[delayedTrain.Name!] = new DelayAnnoucement { delay = delayedTrain.Delay, playlist = trainAnnoucement.playlist.ToArray(), lastPlayed=_currentDelays[delayedTrain.Name!].lastPlayed }!;
                }
            });
        }

        private Task<TrainAnnoucement> PrepareAnnoucementPLaylist(FullCourse delayedTrain)
        {
            TrainAnnoucement trainAnnoucement = new TrainAnnoucement
            {
                playlist = new List<AudioFileReader>()
            };

            if(delayedTrain.ArrivalTime is null)
            {
                _currentDelays.TryRemove(delayedTrain.Name!, out DelayAnnoucement delay);
                return Task.FromResult(trainAnnoucement);
            }
            _logger.LogInformation($"Preparing playlist for delay annoucement: {delayedTrain.Name}");
            trainAnnoucement.playlist = _audioPlaylistService.PrepareAnnoucementPLaylist(delayedTrain).Result;

            return Task.FromResult(trainAnnoucement);
        }


        private async Task SetUpTrainAudioAnnoucementAsync()
        {

            _trainToAnnouce = _annoucementQueueManager.GetTrainAnnoucements();
            if (_trainToAnnouce is not null && _trainToAnnouce.Name is not null)
            {
          
                    await Task.Run(() =>
                    {
                        var task1 = Task.Run(()=> PrepareCoursePlaylist(_trainToAnnouce));
                        if(task1.Result.playlist!.ToArray().Length>0)
                            _annoucementQueueManager.EnqueueReadyAnnoucement(new ConcatenatingSampleProvider(task1.Result.playlist.ToArray()));
                    });

            }

        }


        private async Task SetUpDelayAudioAnnoucementAsync()
        {
            FullCourse delayToAnnouce = _annoucementQueueManager.GetDelayAnnoucements();
            if (delayToAnnouce is not null&&(!_currentDelays.ContainsKey(delayToAnnouce.Name!) || _currentDelays[delayToAnnouce.Name!].delay != delayToAnnouce.Delay))
                await HandleDelayAnnoucement(delayToAnnouce);
        }

        private Task<TrainAnnoucement> PrepareCoursePlaylist(FullCourse course)
        {
            TrainAnnoucement trainAnnoucement = new TrainAnnoucement
            {
                playlist = new List<AudioFileReader>()
            };
            if (_currentDelays.ContainsKey(course.Name!))
            {
                _currentDelays.TryRemove(course.Name!, out DelayAnnoucement delay);
            }
            if (String.IsNullOrEmpty(course.Name!.Split(" ")[0]) 
                || !_audioFileService.GetReadyFileList()["coreFiles"].Contains(course.Name.Split(" ")[0])
                || (int)(TimeOnly.Parse(course.ArrivalTime ?? course.DepartureTime!).AddMinutes(course.Delay==""?0: Double.Parse(course.Delay!)) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes <= 1
                || (int)(TimeOnly.Parse(course.ArrivalTime ?? course.DepartureTime!).AddMinutes(course.Delay == "" ? 0 : Double.Parse(course.Delay!)) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes > 10)
            {
                _logger.LogInformation($"Couse is not suitable for annoucement");
                return Task.FromResult(trainAnnoucement);
            }
            trainAnnoucement = _audioPlaylistService.PrepareCoursePlaylist(course).Result;

            trainAnnoucement.playlist = trainAnnoucement.introduction!.Concat(trainAnnoucement.route!)
                                    .Concat(trainAnnoucement.details!)
                                    .ToList();
            
            return Task.FromResult(trainAnnoucement);

        }

    }
}


/*
{
  "arrivalTime": "22:50",
  "departureTime": "22:51",
  "delay": "",
  "name": "IC 61150",
  "headsignFrom": "Kudowa-Zdrój",
  "headsignTo": "Warszawa Wschodnia",
  "routeFrom": "Kudowa-Zdrój  14:37 -  Wrocław Główny  18:20 -  Oława  18:37 -  Brzeg  18:48 -  Opole Główne  19:13 -  Częstochowa  20:30 •  Koluszki  21:48 -  Skierniewice  22:12 -  Żyrardów  22:26 -  Warszawa Zachodnia  22:50",
  "routeTo": "Warszawa Zachodnia  22:51 -  Warszawa Centralna  22:55 -  Warszawa Wschodnia  23:06",
  "platform": "VI/2"
}
*/
