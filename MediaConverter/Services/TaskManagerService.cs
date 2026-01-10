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
        //private readonly ICloudConvertService _cloudConvertService;
        private readonly IFFmpegConversionService _fFmpegConverionService;

        public TaskManagerService(/*ICloudConvertService cloudConvertService*/ IFFmpegConversionService fFmpegConverionService)
        {
            _fFmpegConverionService = fFmpegConverionService;
            //_cloudConvertService = cloudConvertService;
        }

        public async Task ExecuteTask()
        {
            await _fFmpegConverionService.ProcessFileConversion($"D:\\Code\\MyCode\\StationSchedule\\MediaConverter\\ReklamaPKPIntercityZakochajsięwkolei.mp4");
            return;
        }

    }
}
