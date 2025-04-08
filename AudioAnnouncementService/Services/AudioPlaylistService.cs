using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Builders;
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
    internal sealed class AudioPlaylistService : IAudioPlaylistService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AudioPlaylistService> _logger;
        private readonly IAudioFileService _audioFileService;
        private readonly TrainAnnoucementBuilder _builder;

        public AudioPlaylistService(IConfiguration configuration, ILogger<AudioPlaylistService> logger,IAudioFileService audioFileService,TrainAnnoucementBuilder builder)
        {
            _configuration = configuration;
            _logger = logger;
            _audioFileService = audioFileService;
            _builder = builder;
        }



        public Task<TrainAnnoucement> PrepareCoursePlaylist(FullCourse fullCourse)
        {
            
            _logger.LogInformation($"Preparing playlist for train annoucement: {fullCourse.Name}");

            var trainAnnoucement = _builder
                .SetIntroduction(fullCourse)
                .SetRoute(fullCourse)
                .SetDetails(fullCourse)
                .Build();
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

        
    }



   
}
