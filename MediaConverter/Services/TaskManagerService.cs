using FFMpegCore.Enums;
using MediaConverter.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaConverter.Services
{
    internal sealed class TaskManagerService : ITaskManagerService
    {
        private readonly IFFmpegConversionService _fFmpegConverionService;
        private readonly IMediaConversionService _mediaConversionService;

        public TaskManagerService(IFFmpegConversionService fFmpegConverionService, IMediaConversionService mediaConversionService)
        {

        
            _fFmpegConverionService = fFmpegConverionService;
            _mediaConversionService = mediaConversionService;

        }

        public async Task ExecuteTask()
        {
            //await _fFmpegConverionService
            //await _mediaConversionService.
            await _fFmpegConverionService.ProcessFileConversion($"D:\\Code\\MyCode\\StationSchedule\\MediaConverter\\ReklamaPKPIntercityZakochajsięwkolei.mp4");
            return;
        }

    }
}
