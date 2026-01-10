namespace StationAPI.Models
{
    public class Train
    {
        public string? id { get; set; }
        public string? course_id { get; set; }
        public string? trip_headsign { get; set; }
        public decimal? shape_pt_lat { get; set; }
        public decimal? shape_pt_lon { get; set; }
        public string? stop_name { get; set; }
        public string? arrival_time { get; set; }
        public string? prev_stop_name { get; set; }
        public string? carrier { get; set; }


    }
}
