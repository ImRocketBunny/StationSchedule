using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal interface IMqttManagerService
    {
        Task SetUpMqttClientAsync();
        Task DisposeMqttClientAsync();
        Task PublishSchedule(Dictionary<string, string> keyValuePairs);
    }
}
