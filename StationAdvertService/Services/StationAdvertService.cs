using MediaInfo;
using StationAdvertService.Abstract;
using System.Collections.Concurrent;
using static System.Collections.Specialized.BitVector32;

namespace StationAdvertService.Services
{
    internal sealed class StationAdvertService : IStationAdvertService
    {
        private readonly IMqttClientService _mqttClientService;
        private readonly ILogger<StationAdvertService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAdvertFileService _advertFileService;
        private List<string>? _stationPlatforms;
        private List<string>? _runningTasks = new List<string>();
        private TaskManager _taskManager;
        private IMqttClientService _mqttClient;
        List<string> topics =
["PLK_wylamiane_rogatki_nowe-r20250123-7.webm", "POK_skm_CZARODZIEJSKI-FLET-DLA-DZIECI_03.2025-r20250205-9.webm",
  "Praca_SKM_elektryk_1920x810-r20250116-5.webm", "TS_Mahagonny_1920x810-r20241204-3.webm"
  , "4_UTK_animacja_BAGAZ_NEW-r20250113-5.webm","ZTM_Warszawa_mruga_9.02-r20250203-1.webm","POK_VENUS-AND-ADONIS_25.02-r20250115-3.webm"
,"TS_STARA-1920x810-r20241017-19.webm","ZTM_PLAKAT_E_HOLOGRAM_DLA_HB_TABOR_SKM_2-r20250117-9.webm","SKM_20lecie_1920x810-r20240510-15.webm" ];
        /*private readonly IAdvertRepository() _advertRepository;
        private readonly IStationRepository() _stationRepository;*/

        public StationAdvertService(IMqttClientService mqttClient,
            ILogger<StationAdvertService> logger,
            IConfiguration configuration,
            IAdvertFileService advertFileService
 
            ) 
        {
            _taskManager = new TaskManager();
            _mqttClientService = mqttClient;
            _advertFileService = advertFileService;
            _configuration = configuration;
            _logger = logger;
        }








        public async Task ProcessAdvertManaging()
        {
            //Console.WriteLine("Hello?");
            if (_stationPlatforms is not null) return;
            //Console.WriteLine("Here i am?");
            await GetStationStructure();
            //Console.WriteLine("Here i am 2?");
            //_logger.LogInformation($"Creating task for platform ");
            foreach (string platform in _stationPlatforms!) 
            {
                _logger.LogInformation($"Creating task for platform {platform}");
                if (_taskManager.GetStatus(platform) is null)
                {
                    _taskManager.Register(platform, async () => PlatformAdvertManager(platform));
                    _taskManager.Start(platform);
                    //_logger.LogInformation($"Creating task for platform {platform}");

                }
                else 
                {
                    _taskManager!.Restart(platform);
                }


            }
        }










        static async Task RunPlatformThread(Func<Task> action)
        {
            int playlistSize = 0;
            int currPlaylist = 0;
            while (true)
            {
                await action();
            }

            
        }




        private  async Task PlatformAdvertManager(string platform)
        {
            //_logger.LogInformation($"Creating task for platform {platform}");
            List<string> currentPlaylistContent = _advertFileService.GetCurrentPlaylist();
            //_logger.LogInformation($"Creating task for platform {platform} 2");
            var loop = 0;
            int playlistSize = currentPlaylistContent.Count;
            int currPlaylist = 0;
            while (playlistSize==0)
            {
                _logger.LogWarning($"Playlist size for {platform} is 0, waiting 1 minute for refresh");
                await Task.Delay(60000);
                currentPlaylistContent = _advertFileService.GetCurrentPlaylist();
                playlistSize = currentPlaylistContent.Count;
            }
            await _mqttClientService.PublishPlaylist(currentPlaylistContent, "station" + Path.AltDirectorySeparatorChar + platform + Path.AltDirectorySeparatorChar + "playlist");
            await _mqttClientService.PublishNumber(currPlaylist, "station" + Path.AltDirectorySeparatorChar + platform + Path.AltDirectorySeparatorChar + "number");
            await _mqttClientService.PublishValue(currentPlaylistContent[currPlaylist], "station" + Path.AltDirectorySeparatorChar + platform + Path.AltDirectorySeparatorChar + "fileName");

            while (true)
            {
                //Console.WriteLine($"{platform} {playlistSize}");
                if (loop % 5 == 0)
                {
                    currentPlaylistContent = _advertFileService.GetCurrentPlaylist();
                    await _mqttClientService.PublishPlaylist(currentPlaylistContent, "station" + Path.AltDirectorySeparatorChar + platform + Path.AltDirectorySeparatorChar + "playlist");
                    playlistSize = currentPlaylistContent.Count;
                }
                var currVal = _mqttClientService.GetCurrentBrokerValue("station"+ Path.AltDirectorySeparatorChar + platform+ Path.AltDirectorySeparatorChar + "lcd");


                //_logger.LogInformation(currVal);
                //_logger.LogInformation($"{currentPlaylistContent[currPlaylist]}");
                //_logger.LogInformation($"{_mqttClientService.IsAnnoucement(platform)} at {platform}");
                if (!_mqttClientService.IsAnnoucement("station" + Path.AltDirectorySeparatorChar + platform + Path.AltDirectorySeparatorChar + "lcd"))
                {
                    // var media = new MediaInfoWrapper($"..\\MonitorPlatform\\public\\{topics[number]}", _logger);
                    //await Task.Delay(media.Duration);
                    _logger.LogInformation($"Waiting for annoucement to finist at {platform}");
                    currPlaylist++;
                    if (currPlaylist == playlistSize)
                    {
                        currPlaylist = 0;
                        loop++;
                    }
                    //await _mqttClientService.PublishNumber(currPlaylist, "station"+ Path.AltDirectorySeparatorChar + platform + Path.AltDirectorySeparatorChar+"number");
                    //await _mqttClientService.PublishValue(currentPlaylistContent[currPlaylist], "station" + Path.AltDirectorySeparatorChar + platform + Path.AltDirectorySeparatorChar + "fileName");



                    while (!_mqttClientService.IsAnnoucement("station" + Path.AltDirectorySeparatorChar + platform + Path.AltDirectorySeparatorChar + "lcd"))
                    {
                        await Task.Delay(16);
                    }
                    await _mqttClientService.PublishNumber(currPlaylist, "station" + Path.AltDirectorySeparatorChar + platform + Path.AltDirectorySeparatorChar + "number");
                    await _mqttClientService.PublishValue(currentPlaylistContent[currPlaylist], "station" + Path.AltDirectorySeparatorChar + platform + Path.AltDirectorySeparatorChar + "fileName");

                }
                else
                {
                    
                    try
                    {
                        var media = new MediaInfoWrapper($"D:\\Code\\MyCode\\StationScheduleFront\\StationMonitorPlatform\\public\\{currentPlaylistContent[currPlaylist]}", _logger);
                        _logger.LogInformation($"Playing advert {currentPlaylistContent[currPlaylist]} at {platform} for {media.Duration}");
                        await Task.Delay(media.Duration);
                        currPlaylist++;
                        if (currPlaylist == playlistSize)
                        {
                            currPlaylist = 0;
                            loop++;
                        }
                        await _mqttClientService.PublishNumber(currPlaylist, "station"+ Path.AltDirectorySeparatorChar + platform + Path.AltDirectorySeparatorChar + "number");
                        await _mqttClientService.PublishValue(currentPlaylistContent[currPlaylist], "station" + Path.AltDirectorySeparatorChar + platform + Path.AltDirectorySeparatorChar + "fileName");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
                    

                }

                /*if (currPlaylist == playlistSize)
                {
                    currPlaylist = 0;
                    loop++;
                }*/


            }
            //repo. pobierz playlistę
            //var currVal = _mqttClientService.GetCurrentBrokerValue(platform);
            //if(currVal<>) nowe
            
            //if (_mqttClientService.IsAnnoucement(platform)) return;

            ///if(currPlaylist==playlistSize)

            //await _mqttClientService.PublishNumber(1);
        }

        private Task GetStationStructure()
        {
            _stationPlatforms = _configuration.GetSection("StationConfiguration:StationStructure").Get<List<string>>()!;
            //Console.WriteLine("Here i am 2.5?");
            return Task.CompletedTask;
        }



        private async Task<List<string>> GetAdvertPlaylists()
        {
            return  _advertFileService.GetCurrentPlaylist();
        }
    }


    public class TaskManager
    {
        private ConcurrentDictionary<string, Func<Task>> taskFactories = new();
        private ConcurrentDictionary<string, Task> runningTasks = new();

        public void Register(string key, Func<Task> factory)
        {
            taskFactories[key] = factory;
        }

        public void Start(string key)
        {
            if (taskFactories.TryGetValue(key, out var factory))
            {
                var task = factory();
                runningTasks[key] = task;
            }
        }

        public void Restart(string key)
        {
            Start(key);
        }

        public TaskStatus? GetStatus(string key)
        {
            return runningTasks.TryGetValue(key, out var task) ? task.Status : null;
        }
    }


}
