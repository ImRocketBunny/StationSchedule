using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Abstract
{
    internal interface IAudioFileService
    {
        public ConcurrentDictionary<string, List<string>> GetReadyFileList();

        public Task GetAudioFileList();
        public string CreateTrainNameFilePath(string file);
        public string CreateCoreFilePath(string file);
        public string CreateTrackFilePath(string file);
        public string CreateStationFilePath(string file);
        public string CreateMinutesFilePath(string file);  
        public string CreateHourFilePath(string file);
        public string CreatePlatformFilePath(string file);

    }
}
