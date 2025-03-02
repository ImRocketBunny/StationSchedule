namespace StationAPI.Services
{
    using Google.Protobuf;
    public class GtfsService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string GTFS_RT_URL = "https://api.example.com/vehicle_positions.pb"; // Podmień na prawdziwy URL


        public GtfsService()
        {

        }

        public async Task UpdateGTFSPositions()
        {

        }
    }
}
