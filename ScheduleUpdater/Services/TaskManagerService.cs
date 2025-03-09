using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleUpdater.Services
{
    internal sealed class TaskManagerService : ITaskManagerService
    {
        private readonly ILogger<TaskManagerService> _logger;
        private readonly IFileService _fileService;
        private readonly IFileReaderService _readerService;

        public TaskManagerService(ILogger<TaskManagerService> logger, IFileService fileService,IFileReaderService fileReaderService)
        {  _logger = logger; _fileService = fileService; _readerService = fileReaderService; }

        public async Task Execute()
        {
            try
            {
                //await _fileService.FileServiceManager();
                await _readerService.FileReaderManager("ScheduleFiles\\UnpackedSchedules\\Intercity\\stops.txt");
            }
            catch (Exception ex)
            {
                _logger.LogError("Task manager has thrown an exception: {ExMessage}", ex.Message);
            }

        }
    }
}
