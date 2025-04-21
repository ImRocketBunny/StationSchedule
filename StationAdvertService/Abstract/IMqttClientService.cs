using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationAdvertService.Abstract
{
    public interface IMqttClientService
    {
        Task PublishPlaylist(string payload);
        Task SetUpMqttClientAsync();
        bool IsAnnoucement(string platform);
        ConcurrentDictionary<string, string> GetCurrentBrokerState();
        Task PublishNumber(int number);
    }
}
