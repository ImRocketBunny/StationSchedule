namespace StationAPI.Services
{
    using TransitRealtime;
    using Google.Protobuf;
    using System.Net.Http;
    using Google.Protobuf.WellKnownTypes;

    public class GtfsService : IGtfsService
    {
        private readonly HttpClient _httpClient; 
        private const string GTFS_RT_URL = "https://www3.septa.org/gtfsrt/septarail-pa-us/Vehicle/rtVehiclePosition.pb";
        private List<VehiclePosition> _vehicles = new List<VehiclePosition>();
        private bool _initialized = false;
        public GtfsService()
        {
            _httpClient = new HttpClient();
        }



        public async Task TaskManager()
        {
            if (_initialized) return;
            while (true)
            {
                _initialized = true;
                var vehicles = await UpdateGTFSPositions();

                //Console.Clear();
                Console.WriteLine($"📡 Ostatnia aktualizacja: {DateTime.Now}");
                foreach (var vehicle in vehicles)
                {
                    if(vehicle.Vehicle.Label== "426,425,405,316,317")
                    Console.WriteLine($"ID: {vehicle.Vehicle.Label}, Pojazd: {vehicle.Vehicle.Id}, Kierunek: {vehicle.Trip.DirectionId}, Pozycja: {vehicle.Position.Latitude}, {vehicle.Position.Longitude}, Prędkość: {vehicle.Position.Speed} {vehicle.Position.Bearing} {vehicle.Position.Odometer}");
                }

                await Task.Delay(5000);
            }
        }

        public async Task<List<VehiclePosition>> UpdateGTFSPositions()
        {
            try
            {
                var response = await _httpClient.GetByteArrayAsync(GTFS_RT_URL);
                FeedMessage feed = FeedMessage.Parser.ParseFrom(response);
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds((long)feed.Header.Timestamp).UtcDateTime;
                Console.WriteLine($"📅 Data lokalna: {dateTime.ToLocalTime()}");
                _vehicles = new List<VehiclePosition>();
                foreach (var entity in feed.Entity)
                {
                    if (entity.Vehicle is not null)
                    {
                        
                        _vehicles.Add(entity.Vehicle);
                        
                    }
                }

                return _vehicles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd pobierania GTFS-RT: {ex.Message}");
                return new List<VehiclePosition>();
            }
        }
    }
}
