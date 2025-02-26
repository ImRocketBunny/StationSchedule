using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Services
{
    class AudioPlaylistService : IAudioPlaylistService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AudioPlaylistService> _logger;
        private readonly IAudioFileService _audioFileService;

        public AudioPlaylistService(IConfiguration configuration, ILogger<AudioPlaylistService> logger,IAudioFileService audioFileService)
        {
            _configuration = configuration;
            _logger = logger;
            _audioFileService = audioFileService;
        }



        public Task<TrainAnnoucement> PrepareCoursePlaylist(FullCourse fullCourse)
        {
            
            _logger.LogInformation($"Preparing playlist for train annoucement: {fullCourse.Name}");

            TrainAnnoucement trainAnnoucement = new TrainAnnoucement
            {
                introduction = PrepareIntroduction(fullCourse).Result,
                route = PrepareRoute(fullCourse).Result,
                details = PrepareDetails(fullCourse).Result
            };
            

            /*trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath(fullCourse.Name!.Split(" ")[0])));
            if (fullCourse.Name.Split("   ").Length > 1 && _audioFileService.GetReadyFileList()["trainNames"].Contains(fullCourse.Name.Split("   ")[1].Split("/")[0]))
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateTrainNameFilePath(fullCourse.Name.Split("   ")[1].Split("/")[0])));
            if (fullCourse.HeadsignFrom != "" && _audioFileService.GetReadyFileList()["stationNames"].Contains(fullCourse.HeadsignFrom!))
            {
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("ze_stacji")));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateStationFilePath(fullCourse.HeadsignFrom!)));
            }
            if (fullCourse.HeadsignTo != "" && _audioFileService.GetReadyFileList()["stationNames"].Contains(fullCourse.HeadsignTo!))
            {
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("do_stacji")));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateStationFilePath(fullCourse.HeadsignTo!)));
            }

            if (fullCourse.ArrivalTime is not null)
            {
                if (fullCourse.DepartureTime is not null)
                {
                    var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(fullCourse.RouteTo!, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");
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

                if (fullCourse.DepartureTime is null)
                {
                    var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(fullCourse.RouteFrom!, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");
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
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateHourFilePath(fullCourse.ArrivalTime.Split(":")[0])));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateMinutesFilePath(fullCourse.ArrivalTime.Split(":")[1])));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("wjedzie_na")));
                if (fullCourse.Platform!.Split("/").Length > 1)
                    trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Track\\" + fullCourse.Platform.Split("/")[1].Split(" ")[0] + ".mp3")));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("przy_peronie")));
                trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreatePlatformFilePath(fullCourse.Platform!.Split("/")[0])));

                if (fullCourse.DepartureTime is null)
                {
                    trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("pociąg_konczy")));
                }

            }
            if (fullCourse.ArrivalTime is null && fullCourse.DepartureTime is not null)
            {
                var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(fullCourse.RouteTo, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");

                if (otherStations.Length > 2)
                {
                    trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("przez_stacje")));
                    for (int i = 1; i < otherStations.Length - 1; i++)
                    {
                        try
                        {
                            trainAnnoucement.playlist.Add(new AudioFileReader(".\\Sounds\\Stations\\" + otherStations[i] + ".mp3"));

                        }
                        catch (Exception ex)
                        {

                            _logger.LogError($"{ex.GetType().Name} Station file missing: {otherStations[i]}");

                        }
                    }

                }
                trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Core\\stoi_na.mp3")));
                if (fullCourse.Platform!.Split("/").Length > 1)
                    trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Track\\" + fullCourse.Platform.Split("/")[1].Split(" ")[0] + ".mp3"))); trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Core\\przy_peronie.mp3")));
                trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Platform\\" + fullCourse.Platform.Split("/")[0] + ".mp3")));

                if (fullCourse.Delay == "")
                {
                    trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("planowy_odjazd")));
                    trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateHourFilePath(fullCourse.DepartureTime.Split(":")[0])));
                    trainAnnoucement.playlist.Add(new AudioFileReader(_audioFileService.CreateMinutesFilePath(fullCourse.DepartureTime.Split(":")[1])));
                }
            }


            if (fullCourse.ArrivalTime is null)
            {
                trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Core\\życzymy.mp3")));
            }
            else
            {
                trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Core\\prosimy_zachować.mp3")));

            }


            if (fullCourse.Delay != string.Empty)
                trainAnnoucement.playlist.Add(new AudioFileReader((".\\Sounds\\Core\\za_opóźnienie.mp3")));
            */
            return Task.FromResult(trainAnnoucement);
        }


        public Task<List<AudioFileReader>> PrepareAnnoucementPLaylist(FullCourse delayedTrain)
        {
            List<AudioFileReader> playlist = new List<AudioFileReader>();

            _logger.LogInformation($"Preparing delay annoucement for train: {delayedTrain.Name}");
            playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath(delayedTrain.Name!.Split(" ")[0])));
            if (delayedTrain.Name.Split("   ").Length > 1 && _audioFileService.GetReadyFileList()["trainNames"].Contains(delayedTrain.Name.Split("   ")[1].Split("/")[0]))
                playlist.Add(new AudioFileReader(_audioFileService.CreateTrainNameFilePath(delayedTrain.Name.Split("   ")[1].Split("/")[0])));
            if (delayedTrain.HeadsignFrom != "" && _audioFileService.GetReadyFileList()["stationNames"].Contains(delayedTrain.HeadsignFrom!))
            {
                playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("ze_stacji")));
                playlist.Add(new AudioFileReader(_audioFileService.CreateStationFilePath(delayedTrain.HeadsignFrom!)));
            }
            if (delayedTrain.HeadsignTo != "" && _audioFileService.GetReadyFileList()["stationNames"].Contains(delayedTrain.HeadsignTo!))
            {
                playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("do_stacji")));
                playlist.Add(new AudioFileReader(_audioFileService.CreateStationFilePath(delayedTrain.HeadsignTo!)));
            }
            playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("planowy_przyjazd")));
            playlist.Add(new AudioFileReader(_audioFileService.CreateHourFilePath(delayedTrain.ArrivalTime!.Split(":")[0])));
            playlist.Add(new AudioFileReader(_audioFileService.CreateMinutesFilePath(delayedTrain.ArrivalTime.Split(":")[1])));
            playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("jest_opozniony")));
            playlist.Add(new AudioFileReader(_audioFileService.CreateDelayFilePath(delayedTrain.Delay!)));
            playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("minut")));
            playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("komunikat_opoznienie")));
            return Task.FromResult(playlist);
        }

        private Task<List<AudioFileReader>> PrepareIntroduction(FullCourse fullCourse)
        {
            
            List<AudioFileReader> playlist = new List<AudioFileReader>();


            _logger.LogInformation($"Preparing introduction segment for train: {fullCourse.Name}");
            playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath(fullCourse.Name!.Split(" ")[0])));
            if (fullCourse.Name.Split("   ").Length > 1 && _audioFileService.GetReadyFileList()["trainNames"].Contains(fullCourse.Name.Split("   ")[1].Split("/")[0]))
                playlist.Add(new AudioFileReader(_audioFileService.CreateTrainNameFilePath(fullCourse.Name.Split("   ")[1].Split("/")[0])));
            if (fullCourse.HeadsignFrom != "" && _audioFileService.GetReadyFileList()["stationNames"].Contains(fullCourse.HeadsignFrom!))
            {
                playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("ze_stacji")));
                playlist.Add(new AudioFileReader(_audioFileService.CreateStationFilePath(fullCourse.HeadsignFrom!)));
            }
            if (fullCourse.HeadsignTo != "" && _audioFileService.GetReadyFileList()["stationNames"].Contains(fullCourse.HeadsignTo!))
            {
                playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("do_stacji")));
                playlist.Add(new AudioFileReader(_audioFileService.CreateStationFilePath(fullCourse.HeadsignTo!)));
            }

            return Task.FromResult(playlist);

        }


        private Task<List<AudioFileReader>>PrepareRoute(FullCourse fullCourse)
        {
            List<AudioFileReader> playlist = new List<AudioFileReader>();
            _logger.LogInformation($"Preparing route segment for train: {fullCourse.Name}");

            if (fullCourse.DepartureTime is not null)
            {
                var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(fullCourse.RouteTo!, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");
                if (otherStations.Length > 2)
                {
                    playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("przez_stacje")));
                    for (int i = 1; i < otherStations.Length - 1; i++)
                    {
                        try
                        {
                            playlist.Add(new AudioFileReader(_audioFileService.CreateStationFilePath(otherStations[i])));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Station file missing: {otherStations[i]}");
                        }
                    }

                }
            }else if(fullCourse.ArrivalTime is not null)
            {
                var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(fullCourse.RouteFrom!, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");
                if (otherStations.Length > 2)
                {
                    playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("przez_stacje")));
                    for (int i = 1; i < otherStations.Length - 1; i++)
                    {
                        try
                        {
                            playlist.Add(new AudioFileReader(_audioFileService.CreateStationFilePath(otherStations[i])));

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Station file missing: {otherStations[i]}");
                        }
                    }

                }
            }
            return Task.FromResult(playlist);
        }


        private Task<List<AudioFileReader>> PrepareDetails(FullCourse fullCourse)
        {
            List<AudioFileReader> playlist = new List<AudioFileReader>();
            _logger.LogInformation($"Preparing detail segment for train: {fullCourse.Name}");
            if (fullCourse.ArrivalTime is not null)
            {
                playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("planowy_przyjazd")));
                playlist.Add(new AudioFileReader(_audioFileService.CreateHourFilePath(fullCourse.ArrivalTime.Split(":")[0])));
                playlist.Add(new AudioFileReader(_audioFileService.CreateMinutesFilePath(fullCourse.ArrivalTime.Split(":")[1])));
                playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("wjedzie_na")));
                if (fullCourse.Platform!.Split("/").Length > 1)
                    playlist.Add(new AudioFileReader((".\\Sounds\\Track\\" + fullCourse.Platform.Split("/")[1].Split(" ")[0] + ".mp3")));
                playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("przy_peronie")));
                playlist.Add(new AudioFileReader(_audioFileService.CreatePlatformFilePath(fullCourse.Platform!.Split("/")[0])));

                if (fullCourse.DepartureTime is null)
                {
                    playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("pociąg_konczy")));
                }
                playlist.Add(new AudioFileReader((".\\Sounds\\Core\\prosimy_zachować.mp3")));

            }
            else if (fullCourse.DepartureTime is not null)
            {
                playlist.Add(new AudioFileReader((".\\Sounds\\Core\\stoi_na.mp3")));
                if (fullCourse.Platform!.Split("/").Length > 1)
                    playlist.Add(new AudioFileReader((".\\Sounds\\Track\\" + fullCourse.Platform.Split("/")[1].Split(" ")[0] + ".mp3"))); 
                playlist.Add(new AudioFileReader((".\\Sounds\\Core\\przy_peronie.mp3")));
                playlist.Add(new AudioFileReader((".\\Sounds\\Platform\\" + fullCourse.Platform.Split("/")[0] + ".mp3")));

                if (fullCourse.Delay == "")
                {
                    playlist.Add(new AudioFileReader(_audioFileService.CreateCoreFilePath("planowy_odjazd")));
                    playlist.Add(new AudioFileReader(_audioFileService.CreateHourFilePath(fullCourse.DepartureTime.Split(":")[0])));
                    playlist.Add(new AudioFileReader(_audioFileService.CreateMinutesFilePath(fullCourse.DepartureTime.Split(":")[1])));
                }
                
                playlist.Add(new AudioFileReader((".\\Sounds\\Core\\życzymy.mp3")));
                
            }
            if (fullCourse.Delay != string.Empty)
                playlist.Add(new AudioFileReader((".\\Sounds\\Core\\za_opóźnienie.mp3")));

            return Task.FromResult(playlist);
        }
    }



   
}
