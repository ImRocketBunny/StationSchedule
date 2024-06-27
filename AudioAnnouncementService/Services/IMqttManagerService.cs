using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAnnouncementService.Services
{
    internal interface IMqttManagerService
    {
        Task SetUpMqttClientAsync();
        Task DisposeMqttClientAsync();
        Task GetNextCourseToAnnouce(object value);
    }
}
