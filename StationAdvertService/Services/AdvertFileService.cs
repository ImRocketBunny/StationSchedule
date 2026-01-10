using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StationAdvertService.Abstract;
using StationAdvertService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace StationAdvertService.Services
{
    class AdvertFileService : IAdvertFileService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientService _httpClientService;
        private readonly ILogger<AdvertFileService> _logger;
        private List<string> _currentPlaylist;
        private List<string> _fileList;
        private string _advertStorageLocation;
        private DateTime _scheduleFileSync;
        private int _fileSyncIntrerval;
        public AdvertFileService(IConfiguration configuration, IHttpClientService httpClientService, ILogger<AdvertFileService> logger)
        {
            _configuration = configuration;
            _httpClientService = httpClientService;
            _logger = logger;
            _scheduleFileSync = DateTime.Now;
            this.GetConfigOptions();

        }


        public async Task ProcessFileManaging()
        {
            if (_scheduleFileSync > DateTime.Now) return;
            this.GetFileList();
            await this.SynchronizeAdverts(GetAdvertPlaylist().Result);
            this.ScheduleNextSync();
        }


        private void GetConfigOptions()
        {
            _advertStorageLocation = _configuration["AdvertFileConfiguration:AdvertStorage"]!;
            _logger.LogInformation($"Advert storage location: {_advertStorageLocation}");
            _fileSyncIntrerval = _configuration.GetValue<int>("AdvertFileConfiguration:AdvertSyncIntervalMinutes");
            _logger.LogInformation($"File sync interval: {_fileSyncIntrerval}");
        }

        private void ScheduleNextSync()
        {
            _scheduleFileSync = DateTime.Now.AddMinutes(_fileSyncIntrerval);
            _logger.LogInformation($"Next file synchronization will begin at: {_scheduleFileSync}");
        }

        private void GetFileList()
        {
            _fileList = Directory.GetFiles(_advertStorageLocation).Select(Path.GetFileName).ToList();

        }

        private async Task<AdvertPlaylist> GetAdvertPlaylist()
        {
            string? playlist=null;
            try
            {
                 playlist = await _httpClientService!.GetAdvertsAsync();

            }catch(Exception ex)
            {
                _logger.LogError($"{ex.Message}");
            }
            AdvertContent ac = JsonConvert.DeserializeObject<AdvertContent>(playlist);

            if (playlist is null || ac is null || ac.Playlists is null)
            {
                return new AdvertPlaylist
                {
                    Contents = new List<Advert>()
                };
            }


            return ac.Playlists.Last();

                
        }


        private async Task DownloadFile(string filename)
        {
            await _httpClientService.DownloadFileAsync(filename, _advertStorageLocation);
        }

        private async Task SynchronizeAdverts(AdvertPlaylist advserts)
        {
            if (advserts.Contents is null)
                return;

 
            foreach(Advert ad in advserts.Contents)
            {
                
                
                if (!_fileList.Contains(ad.Filename))
                {
                    try
                    {
                        _logger.LogInformation($"Downloading Advert: {ad.Filename}");
                        await this.DownloadFile(ad.Filename);

                    }catch(Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        _fileList.Remove(ad.Filename);
                    }
                }else
                {
                    _logger.LogInformation($"Advert {ad.Filename} has been downloaded.");
                }
            }
            _currentPlaylist = advserts.Contents.Select(e => e.Filename).Where(f=>f.EndsWith(".webm")).ToList();
            Console.WriteLine();
        }

        public List<string> GetCurrentPlaylist()
        {
            return _currentPlaylist;
        }


    }
}
