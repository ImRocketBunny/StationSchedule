using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationScheduleService.Services
{
    internal class MqttManagerService : IMqttManagerService
    {
        public Task DisposeMqttClientAsync()
        {
            throw new NotImplementedException();
        }

        public Task PublishSchedule(object value)
        {
            throw new NotImplementedException();
        }

        public Task SetUpMqttClientAsync()
        {
            throw new NotImplementedException();
        }
    }
}
