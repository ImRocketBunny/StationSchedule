using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Abstract
{
    internal interface IMqttManagerService
    {
        Task SetUpMqttClientAsync();
        Task DisposeMqttClientAsync();
        Task GetNextCourseToAnnouce(object value);
        Task ReceiveMqttDataAsync(BlockingCollection<string> messageQueue);
    }
}
