using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Models
{
    internal class FullCourse
    {
        [JsonProperty("arrivalTime")]
        public string? ArrivalTime { get; set; }
        [JsonProperty("departureTime")]
        public string? DepartureTime { get; set; }
        [JsonProperty("delay")]
        public string? Delay { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("headsignFrom")]
        public string? HeadsignFrom { get; set; }
        [JsonProperty("headsignTo")]
        public string? HeadsignTo { get; set; }
        [JsonProperty("routeFrom")]
        public string? RouteFrom { get; set; }
        [JsonProperty("routeTo")]
        public string? RouteTo { get; set; }
        [JsonProperty("platform")]
        public string? Platform { get; set; }

    }
}
