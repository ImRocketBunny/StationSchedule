using FFMpegCore;
using MediaConverter.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaConverter.Services
{
    internal class FFmpegConversionService : IFFmpegConversionService
    {
        private readonly ILogger<FFmpegConversionService> _logger;
        public FFmpegConversionService(ILogger<FFmpegConversionService> logger)
        {
            _logger = logger;
        }




        public async Task ProcessFileConversion(string inputPath)
        {
            await AnalizeFile(inputPath);
        }


        private async Task AnalizeFile(string inputPath)
        {
            var mediaInfo = await FFProbe.AnalyseAsync(inputPath);
            _logger.LogInformation($"{mediaInfo.Duration} {mediaInfo.PrimaryVideoStream.FrameRate} {mediaInfo.PrimaryVideoStream.Index} {mediaInfo.VideoStreams.Count}");
        }











    }
}
