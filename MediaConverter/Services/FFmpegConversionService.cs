using FFMpegCore;
using FFMpegCore.Enums;
using MediaConverter.Models;
using MediaConverter.Services.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaConverter.Services
{
    internal class FFmpegConversionService : IFFmpegConversionService
    {
        private readonly ILogger<FFmpegConversionService> _logger;
        private readonly IConfiguration _configuration;

        private int _forcedFrameRate;
        private string _sourceDirectory;
        private string _destinationDirectory;
        private bool _useForcedFramerate = true;
        private List<string> _compatibleImageFormats;
        private List<string> _compatibleVideoFormats;

        public FFmpegConversionService(ILogger<FFmpegConversionService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        public async Task SetupConverter()
        {
            _sourceDirectory = _configuration["File:FileSourcePath"]!;
            _destinationDirectory = _configuration["File:FileDestinationPath"]!;
            _forcedFrameRate = _configuration.GetValue<int>("Conversion:Framerate")!;
            _useForcedFramerate = _forcedFrameRate == 0 ? false : true;
            _compatibleImageFormats = _configuration.GetSection("Conversion:ImageFormats").Get<List<string>>()!;
            _compatibleVideoFormats = _configuration.GetSection("Conversion:VideoFormats").Get<List<string>>()!;
        }




        public async Task ProcessFileConversion(string inputPath)
        {
            var info = await FFProbe.AnalyseAsync(inputPath);
            bool isVideo = info.VideoStreams.Any();

            if (isVideo && _compatibleVideoFormats.Contains(info.Format.FormatName))
            {
                await ProcessVideoConversion(inputPath, info);
            }
            else if (!isVideo && _compatibleImageFormats.Contains(info.Format.FormatName))
            {
                await ProcessImageConversion(inputPath);
            }
            {

            }

                
        }


        private async Task<MediaData> AnalizeVideoFile(IMediaAnalysis mediaAnalysis)
        {
           // var mediaInfo = await FFProbe.AnalyseAsync(inputPath);
            _logger.LogInformation($"{mediaAnalysis.Duration} {mediaAnalysis.PrimaryVideoStream.FrameRate} {mediaAnalysis.PrimaryVideoStream.Index} {mediaAnalysis.VideoStreams.Count}");

            var mediaData = new MediaData();

            mediaData.setDuration(mediaAnalysis.Duration);
            mediaData.setFrameRate(mediaAnalysis.PrimaryVideoStream.FrameRate);
            mediaData.setPrimaryVideoIndex(mediaAnalysis.PrimaryVideoStream.Index);


            return mediaData;

        }




        private async Task ProcessVideoConversion(string fileName, IMediaAnalysis mediaAnalysis)
        {
            var mediaData = await AnalizeVideoFile(mediaAnalysis);
            var targetFps = _useForcedFramerate == true? _forcedFrameRate: mediaData.getFrameRate();

            await FFMpegArguments
                .FromFileInput($"{fileName}")
                .OutputToFile("output.webm", overwrite: true, options => options
                    .WithCustomArgument($"-map 0:{mediaData.getPrimaryVideoIndex()}")
                    .WithCustomArgument("-an")
                    .WithCustomArgument($"-r {targetFps}")
                    //.WithVideoFilters(f => f.(targetFps))
                    .WithVideoCodec("libvpx-vp9")
                    .WithConstantRateFactor(30)
                    .WithSpeedPreset(Speed.Slow)
                    .WithCustomArgument("-b:v 0")
                )
                .ProcessAsynchronously();


        }


        private async Task ProcessImageConversion(string fileNameq)
        {

        }
            






    }
}
