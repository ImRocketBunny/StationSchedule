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
        string? GetCurrentBrokerValue(string topic);
        Task PublishNumber(int number);
        Task PublishNumber(int number, string platform);
        Task PublishPlaylist(List<string> playlist, string topic);

        Task PublishValue(string value, string platform);
    }
}
