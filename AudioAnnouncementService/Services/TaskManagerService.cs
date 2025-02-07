using AudioAnnouncementService.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Services
{
    internal sealed class TaskManagerService : ITaskManagerService
    {
        private readonly ILogger<TaskManagerService> _logger;
        private readonly IAudioService _audioService;
        private readonly IMqttManagerService _mqttManagerService;
        private readonly IAudioFileService _audioFileService;

        public TaskManagerService(ILogger<TaskManagerService> logger, IAudioService audioService, IMqttManagerService mqttManagerService, IAudioFileService audioFileService)
        {
            _logger = logger;
            _audioService = audioService;
            _mqttManagerService = mqttManagerService;
            _audioFileService = audioFileService;
        }
        public async Task Execute()
        {
            try { 
                _audioFileService.GetAudioFileList();
                await _mqttManagerService.SetUpMqttClientAsync();
                await _audioService.RunAudioService();


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

        }
    }
}
