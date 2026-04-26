using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace TrainSetService.Services
{
    internal sealed class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private List<string> _fileList;
        private ConcurrentDictionary<string, string[]> _trainSets;


        private string _contentPath;
        private DateTime _scheduledUpdate;
        private int _updateInterval;
        
        public FileService(ILogger<FileService> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory) 
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            
        }


        public void SteupFileManager()
        {
            if (_trainSets is null || _fileList is null)
            {
                _trainSets = new ConcurrentDictionary<string, string[]>();
                LoadConfiguration();
                _scheduledUpdate = DateTime.Now;
            }
            if(_httpClient.BaseAddress is null)
            {
                _httpClient.BaseAddress = new Uri(_configuration["ScrapperSettings:ImageDownload"]!);
                _httpClient.Timeout = TimeSpan.FromSeconds(_configuration.GetValue<int>("ScrapperSettings:TimeOut"));
                _httpClient.DefaultRequestHeaders.Accept.Clear();
            }
            if (_scheduledUpdate < DateTime.Now)
            {
                UpdateAvaliableFiles();
                _scheduledUpdate = _scheduledUpdate.AddMinutes(_updateInterval);
            }
        }



        private void LoadConfiguration()
        {
            _updateInterval = int.Parse(_configuration["FileSettings:UpdateInterval"]!);
            _contentPath = _configuration["FileSettfings:FileDir"]!;
            string[] setsArray = Directory.GetDirectories(_contentPath + _configuration["FileSettings:ReadySets"]!);
            _fileList = Directory.GetFiles(_contentPath).Where(e=>e.EndsWith(".png")).ToList();
            foreach (string setName in setsArray)
            {
                string[] sets = Directory.GetFiles(_contentPath + _configuration["FileSettings:ReadySets"]! + setName);
                _trainSets.TryAdd(setName, sets);
                
            }
            
        }


        private void UpdateAvaliableFiles()
        {
            _fileList = Directory.GetFiles(_contentPath).Where(e => e.EndsWith(".png")).ToList();
            string[] setsArray = Directory.GetDirectories(_contentPath + _configuration["FileSettings:ReadySets"]!);
            foreach (string setName in setsArray)
            {
                string[] sets = Directory.GetFiles(_contentPath + _configuration["FileSettings:ReadySets"]! + setName);
                _trainSets.AddOrUpdate(setName, sets,(key,oldValue)=>sets);

            }

        }




        private async Task DownloadFile(string fileName)
        {
           await _httpClient.GetAsync(_configuration["ScrapperSettings:ImageDownload"]!.Replace("{imageName}",fileName));
        }






    }
}
