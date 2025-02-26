using Newtonsoft.Json;

namespace StationAPI.Models
{
    internal class Course
    {
        [JsonProperty("time")]
        public string? Time { get; set; }
        [JsonProperty("delay")]
        public string? Delay { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("headsign")]
        public string? Headsign { get; set; }
        [JsonProperty("route")]
        public string? Route { get; set; }
        [JsonProperty("platform")]
        public string? Platform { get; set; }
        /*[JsonProperty("deatils")]
        public string? Details { get; set; }*/
        
    }
}
