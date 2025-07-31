using MediaInfo;
using StationAdvertService.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StationAdvertService.Services
{
    internal sealed class AdvertService
    {
        private readonly IMqttClientService _mqttClientService;
        private readonly ILogger<AdvertService> _logger;
        private readonly IConfiguration _configuration;
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

        public AdvertService(IMqttClientService mqttClient,
            ILogger<AdvertService> logger,
            IConfiguration configuration
 
            ) 
        {
            _taskManager = new TaskManager();
            _mqttClient = mqttClient;
        }








        public async Task ProcessAdvertManaging()
        {
            if (_stationPlatforms is not null) return;

            await GetStationStructure();
            foreach(string platform in _stationPlatforms!) 
            {
                if (_taskManager.GetStatus(platform) is null)
                {
                    _taskManager.Register(platform, async () => PlatformAdvertManager(platform));
                    _taskManager.Start(platform);
                }else if (/*_taskManager.GetStatus(platform) is not null*/false)
                {
                    _taskManager!.Restart(platform);
                }
                    //RunPlatformThread(()=>);

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
            int playlistSize = 0;
            int currPlaylist = 0;

            while (true)
            {
               // List<string> playlist= _
                var currVal = _mqttClientService.GetCurrentBrokerValue(platform);

                if (!_mqttClientService.IsAnnoucement(platform))
                {
                   // var media = new MediaInfoWrapper($"..\\MonitorPlatform\\public\\{topics[number]}", _logger);
                    //await Task.Delay(media.Duration);
                    currPlaylist++;
                    await _mqttClientService.PublishNumber(currPlaylist,platform + Path.AltDirectorySeparatorChar+"number");
                    while (!_mqttClientService.IsAnnoucement(platform))
                    {
                        await Task.Delay(33);
                    }
                }
                else
                {
                    var media = new MediaInfoWrapper($"..\\MonitorPlatform\\public\\{topics[currPlaylist]}", _logger);
                    await Task.Delay(media.Duration);
                    currPlaylist++;
                    await _mqttClientService.PublishNumber(currPlaylist, platform + Path.AltDirectorySeparatorChar + "number");

                }

                if (currPlaylist == playlistSize)
                {
                    currPlaylist = 0;
                }


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
            return Task.CompletedTask;
        }



        private async Task GetAdvertPlaylists()
        {
            /*
             * 
             * IAdvertRepostory
             * 
             */
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
