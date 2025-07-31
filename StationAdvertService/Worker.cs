using StationAdvertService.Abstract;
using MediaInfo;
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
            List<string> topics =
["PLK_wylamiane_rogatki_nowe-r20250123-7.webm", "POK_skm_CZARODZIEJSKI-FLET-DLA-DZIECI_03.2025-r20250205-9.webm",
  "Praca_SKM_elektryk_1920x810-r20250116-5.webm", "TS_Mahagonny_1920x810-r20241204-3.webm"
  , "4_UTK_animacja_BAGAZ_NEW-r20250113-5.webm","ZTM_Warszawa_mruga_9.02-r20250203-1.webm","POK_VENUS-AND-ADONIS_25.02-r20250115-3.webm"
,"TS_STARA-1920x810-r20241017-19.webm","ZTM_PLAKAT_E_HOLOGRAM_DLA_HB_TABOR_SKM_2-r20250117-9.webm","SKM_20lecie_1920x810-r20240510-15.webm" ];

            int number = 0;
            //int[] numbers = [48, 15, 15,15,30,14,15,15,10,15];
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
                //Console.WriteLine()
                //Console.WriteLine(GetVideoDuration(topics[number]));
                var media = new MediaInfoWrapper($"..\\MonitorPlatform\\public\\{topics[number]}",_logger);
                _logger.LogInformation($"Playing advert {topics[number]}...");
                await Task.Delay(media.Duration);
                number++;
                if (number==topics.Count-1)
                {
                    number = 0;
                }
            }
        }

        /*public static TimeSpan GetVideoDuration1(string filePath)
        {

            var media = new MediaInfoWrapper(filePath);
            Console.WriteLine(media.Duration);
        }*/

        private static TimeSpan GetVideoDuration(string filePath)
        {
           
            /*using (var shell = ShellObject.FromParsingName($"D:\\Code\\SS\\MonitorPlatform\\public\\{filePath}"))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                var t = (ulong)prop.ValueAsObject;
                return TimeSpan.FromTicks((long)t);
            }*/

            /*Shell shell = new Shell($"D:\\Code\\SS\\MonitorPlatform\\public\\{filePath}");
            string folderPath = System.IO.Path.GetDirectoryName(filePath);
            Folder folder = shell.NameSpace(folderPath);
            FolderItem item = folder.ParseName(System.IO.Path.GetFileName(filePath));

            // Indeks 27 to zazwyczaj "Duration" w systemie Windows
            string duration = folder.GetDetailsOf(item, 27);

            return duration;*/
            return System.TimeSpan.FromHours(1);
        }



    }
}
