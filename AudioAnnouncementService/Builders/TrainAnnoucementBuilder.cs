using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Builders
{
    internal class TrainAnnoucementBuilder
    {
        private TrainAnnoucement _trainAnnoucement=new TrainAnnoucement();
        private readonly IAudioFileService _audioFileService;
        private readonly ILogger<TrainAnnoucementBuilder> _logger;   

        public TrainAnnoucementBuilder(IAudioFileService audioFileService, ILogger<TrainAnnoucementBuilder> logger)
        {
            _audioFileService = audioFileService;
            _logger = logger;
        }


        public TrainAnnoucementBuilder SetIntroduction(FullCourse fullCourse)
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

            _trainAnnoucement.introduction = playlist;
            return this;

        }



        public TrainAnnoucementBuilder SetRoute(FullCourse fullCourse)
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
            }
            else if (fullCourse.ArrivalTime is not null)
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
            _trainAnnoucement.route=playlist;
            return this;
        }


        public TrainAnnoucementBuilder SetDetails(FullCourse fullCourse)
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

            _trainAnnoucement.details = playlist;
            return this;
        }

        public TrainAnnoucement Build() => _trainAnnoucement;


    }
}
