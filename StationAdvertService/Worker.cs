using StationAdvertService.Abstract;

namespace StationAdvertService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMqttClientService _mqttClient;

        public Worker(ILogger<Worker> logger, IMqttClientService mqttClient)
        {
            _logger = logger;
            _mqttClient = mqttClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int number = 0;
            int[] numbers = [48, 15, 15,15,30,14,15,15,10,15];
            await _mqttClient.SetUpMqttClientAsync();
            await _mqttClient.PublishPlaylist("[\"PLK_wylamiane_rogatki_nowe-r20250123-7.webm\"," +
                " \"POK_skm_CZARODZIEJSKI-FLET-DLA-DZIECI_03.2025-r20250205-9.webm\",\r\n" +
                "  \"Praca_SKM_elektryk_1920x810-r20250116-5.webm\", " +
                "\"TS_Mahagonny_1920x810-r20241204-3.webm\"\r\n  ," +
                " \"4_UTK_animacja_BAGAZ_NEW-r20250113-5.webm\"," +
                "\"ZTM_Warszawa_mruga_9.02-r20250203-1.webm\"," +
                "\"POK_VENUS-AND-ADONIS_25.02-r20250115-3.webm\"\r\n," +
                "\"TS_STARA-1920x810-r20241017-19.webm\"," +
                "\"ZTM_PLAKAT_E_HOLOGRAM_DLA_HB_TABOR_SKM_2-r20250117-9.webm\"," +
                "\"SKM_20lecie_1920x810-r20240510-15.webm\" ]");
            while (!stoppingToken.IsCancellationRequested)
            {
                await _mqttClient.PublishNumber(number);
                await Task.Delay(numbers[number]*1000);
                number++;
                if (number == 10)
                {
                    number = 0;
                }
            }
        }
    }
}
