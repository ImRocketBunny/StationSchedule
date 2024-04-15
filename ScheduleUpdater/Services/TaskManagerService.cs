using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleUpdater.Services
{
    internal sealed class TaskManagerService : ITaskManagerService
    {
        private readonly ILogger<TaskManagerService> _logger;

        public TaskManagerService(ILogger<TaskManagerService> logger) {  _logger = logger; }

        public async Task Execute()
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError("Task manager has thrown an exception: {ExMessage}", ex.Message);
            }

        }
    }
}
