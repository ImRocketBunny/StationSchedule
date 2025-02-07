using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Models;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Services
{
    internal class AudioService : IAudioService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AudioService> _logger;
        private readonly IMqttManagerService _mqttManagerService;
        private readonly IAnnoucementQueueManager _annoucementQueueManager;
        private readonly IAudioFileService _audioFileService;
        private FullCourse _trainToAnnouce;
        private FullCourse _delayToAnnouce;
        private WasapiOut _audioVave;

        public AudioService(IConfiguration configuration, 
            ILogger<AudioService> logger, 
            IMqttManagerService mqttManager,
            IAnnoucementQueueManager annoucementQueueManager, 
            IAudioFileService audioFileService) 
        {
            _configuration = configuration;
            _logger = logger;
            _mqttManagerService = mqttManager;
            _annoucementQueueManager = annoucementQueueManager;
            _audioFileService = audioFileService;   
            _audioVave = new WasapiOut();
        }

        public async Task RunAudioService()
        {
            await SetUpAudioAnnoucementAsync();
            await Play();
        }
        private async Task Play()
        {
            if (_annoucementQueueManager.HasReadyAnnoucement())
            {
                var audioVave= new WasapiOut();
                audioVave.Init(_annoucementQueueManager.GetReadyAnnoucement());
                audioVave.Play();
                while (audioVave.PlaybackState == PlaybackState.Playing)
                {
                    await Task.Delay(1000);
                }
                audioVave.Dispose();
            }

            /*while (_audioVave.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(1000);
            }
            _audioVave.Init(playlist);
            _audioVave.Play();
            while (_audioVave.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(1000);
            }
            _audioVave.Dispose();
            _audioVave = new WasapiOut();*/
        }

        private Announcement PrepareAnnoucementPLaylist(Announcement annoucement)
        {
            throw new NotImplementedException();
        }


        private async Task SetUpAudioAnnoucementAsync()
        {

            _trainToAnnouce = _annoucementQueueManager.GetTrainAnnoucements();
            _delayToAnnouce = _annoucementQueueManager.GetDelayAnnoucements();
            if (_trainToAnnouce is not null && _trainToAnnouce.Name is not null)
            {
              
                    await Task.Run(() =>
                    {
                        var task1 = Task.Run(()=> PrepareCoursePlaylist(_trainToAnnouce));
                        _annoucementQueueManager.EnqueueReadyAnnoucement(new ConcatenatingSampleProvider(task1.Result.playlist.ToArray()));
                    });

            }
            if(_delayToAnnouce is not null)
            {
                
            }

        }

        private static async Task RunPeriodicAnnoucement(TimeSpan interval, Func<Task> action)
        {
            while (true)
            {
                await action();
                await Task.Delay(interval);
            }
        }

        private Task <TrainAnnoucement> PrepareCoursePlaylist(FullCourse course)
        {
            TrainAnnoucement trainAnnoucement = new TrainAnnoucement
            { 
                playlist= new List<AudioFileReader>()
            };
            if ((String.IsNullOrEmpty(course.Name!.Split(" ")[0]) || (course.Name!.Split(" ")[0] != string.Empty
                    && !_audioFileService.GetReadyFileList()["coreFiles"].Contains(course.Name.Split(" ")[0]))))
            {
                return Task.FromResult(trainAnnoucement);
            }
            trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath(course.Name!.Split(" ")[0])));
            if (course.Name.Split("   ").Length > 1 && _audioFileService.GetReadyFileList()["trainNames"].Contains(course.Name.Split("   ")[1].Split("/")[0]))
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateTrainNameFilePath(course.Name.Split("   ")[1].Split("/")[0])));
            if (course.HeadsignFrom != "" && _audioFileService.GetReadyFileList()["stationNames"].Contains(course.HeadsignFrom!))
            {
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("ze_stacji")));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateStationFilePath(course.HeadsignFrom!)));
            }
            if (course.HeadsignTo != "" &&  _audioFileService.GetReadyFileList()["stationNames"].Contains(course.HeadsignTo!))
            {
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("do_stacji")));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateStationFilePath(course.HeadsignTo!)));
            }

            if (course.ArrivalTime is not null)
            {
                if (course.DepartureTime is not null)
                {
                    var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(course.RouteTo!, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");
                    if (otherStations.Length > 2)
                    {
                        trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("przez_stacje")));
                        for (int i = 1; i < otherStations.Length - 1; i++)
                        {
                            try
                            {
                                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateStationFilePath(otherStations[i])));
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Station file missing: {otherStations[i]}");
                            }
                        }

                    }
                }

                if (course.DepartureTime is null)
                {
                    var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(course.RouteFrom!, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");
                    if (otherStations.Length > 2)
                    {
                        trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("przez_stacje")));
                        for (int i = 1; i < otherStations.Length - 1; i++)
                        {
                            try
                            {
                                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateStationFilePath(otherStations[i])));

                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Station file missing: {otherStations[i]}");
                            }
                        }

                    }
                }

                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("planowy_przyjazd")));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateHourFilePath(course.ArrivalTime.Split(":")[0])));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateMinutesFilePath(course.ArrivalTime.Split(":")[1])));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("wjedzie_na")));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("przy_peronie")));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreatePlatformFilePath(course.Platform!.Split("/")[0])));

                if (course.DepartureTime is null)
                {
                    trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("pociąg_konczy")));
                }
                else
                {
                    if (course.ArrivalTime != course.DepartureTime)
                    {
                        trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("planowy_odjazd")));
                        trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateHourFilePath(course.DepartureTime.Split(":")[0])));
                        trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateMinutesFilePath(course.DepartureTime.Split(":")[1])));
                    }
                    /*lista.Add(new AudioFileReader((".\\Sounds\\Core\\planowy_odjazd.mp3")));
                    lista.Add(new AudioFileReader((".\\Sounds\\Time\\Hours\\" + (courses.DepartureTime).Split(":")[0] + ".mp3")));
                    lista.Add(new AudioFileReader((".\\Sounds\\Time\\Minutes\\" + (courses.DepartureTime).Split(":")[1] + ".mp3")));*/

                }
            }
            if (course.ArrivalTime is null && course.DepartureTime is not null)
            {
                var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(course.RouteTo, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");

                if (otherStations.Length > 2)
                {
                    trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("przez_stacje")));
                    for (int i = 1; i < otherStations.Length - 1; i++)
                    {
                        try
                        {
                            trainAnnoucement.playlist.Add(new AudioFileReader(".\\Sounds\\Stations\\" + otherStations[i] + ".mp3"));
                            Console.Write(otherStations[i] + ", ");
                        }
                        catch (Exception ex)
                        {
                            Console.Write(otherStations[i] + ", ");
                            //Console.WriteLine(ex.ToString());
                        }
                    }

                }
                trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Core\\stoi_na.mp3")));
                trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Core\\przy_peronie.mp3")));
                trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Platform\\" + course.Platform.Split("/")[0] + ".mp3")));

                if (course.Delay == "")
                {
                    trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Core\\planowy_odjazd.mp3")));
                    trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Time\\Hours\\" + (course.DepartureTime).Split(":")[0] + ".mp3")));
                    trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Time\\Minutes\\" + (course.DepartureTime).Split(":")[1] + ".mp3")));
                }
            }

            /*try
            {
                lista.Add(new AudioFileReader((".\\Sounds\\Time\\Minutes\\" + (c.ArrivalTime ?? c.DepartureTime).Split(":")[1] + ".mp3")));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }*/
            //lista.Add(new AudioFileReader((".\\Sounds\\Time\\Hours\\"+c.Time.Split(":")[0]+".mp3")));
            //lista.Add(new AudioFileReader((".\\Sounds\\Time\\Minutes\\" + c.Time.Split(":")[1] + ".mp3")));
            //lista.Add(new AudioFileReader((".\\Sounds\\Core\\prosimy_zachować.mp3")));

            if (course.ArrivalTime == null)
            {
                trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Core\\życzymy.mp3")));
            }
            else
            {
                trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Core\\prosimy_zachować.mp3")));

            }


            if (course.Delay != string.Empty)
                trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Core\\za_opóźnienie.mp3")));
            /*if (course.HeadsignTo != "" && course.HeadsignFrom != "" && FileList3.Contains(course.HeadsignFrom) && FileList3.Contains(courses.HeadsignTo))
            {
                lista.Add(new AudioFileReader((".\\Sounds\\Core\\ze_stacji.mp3")));
                lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + courses.HeadsignFrom + ".mp3"));
                lista.Add(new AudioFileReader((".\\Sounds\\Core\\do_stacji.mp3")));
                lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + courses.HeadsignTo + ".mp3"));
            }*/
            return Task.FromResult(trainAnnoucement);

        }

    }
}
