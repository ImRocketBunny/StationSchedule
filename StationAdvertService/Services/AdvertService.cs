using StationAdvertService.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationAdvertService.Services
{
    internal sealed class AdvertService
    {
        private readonly IMqttClientService _mqttClientService;
        private readonly ILogger<AdvertService> _logger;
        /*private readonly IAdvertRepository() _advertRepository;
        private readonly IStationRepository() _stationRepository;*/

        public AdvertService(IMqttClientService mqttClientService, ILogger<AdvertService> logger) { }



        static async Task RunPeriodicTask(Func<Task> action)
        {
            while (true)
            {
                await action();

            }
        }


        private async Task PlatformAdvertManager(string platform)
        {
            if (_mqttClientService.IsAnnoucement(platform)) return;
            await _mqttClientService.PublishPlaylist("playlist");
            await _mqttClientService.PublishNumber(1);
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
}
