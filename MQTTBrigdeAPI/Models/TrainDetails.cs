using System;

namespace StationAPI.Models
{
    public class TrainDetails
    {
        public int stop_sequence {  get; set; }
        public string? stop_id { get; set; }
        public string? stop_name { get; set; }
        public decimal stop_lon { get; set; }
        public decimal stop_lat { get; set; }
        public string? arrival_time { get; set; }
        public string?  departure_time { get; set; }
        public string? trip_headsign { get; set; }
        public string? plk_train_number { get; set; }
    }
}
