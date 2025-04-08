using StationAPI.Abstract.DAL;

namespace StationAPI.Services
{
    public class StationAdvertService : IStationAdvertService
    {
        private readonly IConfiguration _configuration;
        private readonly IApiRepository _apiRepository;
        private readonly IMqttClientService _mqttClientService;

        public StationAdvertService() { }




    }
}
