using AudioAnnouncementService.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Services
{
    internal class AudioFileService : IAudioFileService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AudioFileService> _logger;
        private DirectoryInfo _trackNames;
        private DirectoryInfo _stationNames;
        private DirectoryInfo _trainNames;
        private DirectoryInfo _platformNames;
        private DirectoryInfo _coreFiles;
        public ConcurrentDictionary<string, List<string>> _files;   

        public AudioFileService(IConfiguration configuration, ILogger<AudioFileService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _trackNames = new DirectoryInfo(_configuration["FilePaths:TrackNames"]!);
            _stationNames = new DirectoryInfo(_configuration["FilePaths:StationNames"]!);
            _trainNames = new DirectoryInfo(_configuration["FilePaths:TrainNames"]!);
            _platformNames = new DirectoryInfo(_configuration["FilePaths:PlatrfomNames"]!);
            _coreFiles = new DirectoryInfo(_configuration["FilePaths:CoreFiles"]!);
            _files = new ConcurrentDictionary<string, List<string>>();

        }


        public Task GetAudioFileList()
        {
            GetCoreFiles();
            GetStationList();
            GetTrainNamesList();
            GetTrackNamesList();
            GetPlatformNames();
            return Task.CompletedTask;
        }

        public ConcurrentDictionary<string, List<string>> GetReadyFileList()
        {
            return _files;
        }

        private Task GetStationList()
        {
            List<string> stationFileList = _stationNames.GetFiles("*.mp3").Select(e => e.Name.Replace(".mp3", "")).ToList();
            if (!_files.ContainsKey("stationNames"))
                _files.TryAdd("stationNames", stationFileList);
            else
                _files["stationNames"] = stationFileList;

            return Task.CompletedTask;
        }

        private Task GetTrainNamesList()
        {
            List<string> trainNamesList = _trainNames.GetFiles("*.mp3").Select(e => e.Name.Replace(".mp3", "")).ToList();
            if (!_files.ContainsKey("trainNames"))
                _files.TryAdd("trainNames", trainNamesList);
            else
                _files["trainNames"] = trainNamesList;

            return Task.CompletedTask;
        }

        private Task GetTrackNamesList()
        {
            List<string> trackNamesList = _trackNames.GetFiles("*.mp3").Select(e => e.Name.Replace(".mp3", "")).ToList();
            if (!_files.ContainsKey("trackNames"))
                _files.TryAdd("trackNames", trackNamesList);
            else
                _files["trackNames"] = trackNamesList;

            return Task.CompletedTask;
        }

        private Task GetPlatformNames()
        {
            List<string> platformNamesList = _platformNames.GetFiles("*.mp3").Select(e => e.Name.Replace(".mp3", "")).ToList();
            if (!_files.ContainsKey("platformNames"))
                _files.TryAdd("platformNames", platformNamesList);
            else
                _files["platformNames"] = platformNamesList;

            return Task.CompletedTask;  
        }

        private Task GetCoreFiles()
        {
            List<string> coreFileList = _coreFiles.GetFiles("*.mp3").Select(e => e.Name.Replace(".mp3", "")).ToList();
            if (!_files.ContainsKey("coreFiles"))
                _files.TryAdd("coreFiles", coreFileList);
            else
                _files["coreFiles"] = coreFileList;

            return Task.CompletedTask;
        }


        public string CreateCoreFilePath(string file)
        {
            return _configuration["FilePaths:CoreFiles"]!+file+".mp3";
        }

        public string CreateTrackFilePath(string file)
        {
            return _configuration["FilePaths:TrackNames"]! + file + ".mp3";
        }

        public string CreateTrainNameFilePath(string file)
        {
            return _configuration["FilePaths:TrainNames"]! + file + ".mp3";
        }

        public string CreateStationFilePath(string file)
        {
            return _configuration["FilePaths:StationNames"]! + file + ".mp3";
        }

        public string CreatePlatformFilePath(string file)
        {
            return _configuration["FilePaths:PlatrfomNames"]! + file + ".mp3";
        }

        public string CreateHourFilePath(string file)
        {
            return _configuration["FilePaths:TimeHours"]! + file + ".mp3";
        }

        public string CreateMinutesFilePath(string file)
        {
            return _configuration["FilePaths:TimeMinutes"]! + file + ".mp3";
        }


    }
}
