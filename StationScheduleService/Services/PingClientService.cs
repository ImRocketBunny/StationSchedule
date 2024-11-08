using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal class PingClientService : IPingClientService
    {
        public Ping _ping;
        private readonly ILogger<PingClientService> _logger;

        public PingClientService(ILogger<PingClientService> logger)
        {
            _logger = logger;
            _ping = new Ping();
        }
        
        public async Task<bool> SendPing(string url)
        {
            _logger.LogInformation("Pinging...");
            var result = await _ping.SendPingAsync(url);
            _logger.LogInformation(result.Status.ToString());
            return result.Status == IPStatus.Success;

        }
    }
}
