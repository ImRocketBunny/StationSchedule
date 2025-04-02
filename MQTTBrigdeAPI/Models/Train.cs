namespace StationAPI.Models
{
    public class Train
    {
        public int id { get; set; }
        public string? course_id { get; set; }
        public string? trip_headsign { get; set; }
        public Position? position { get; set; }
    }
}
