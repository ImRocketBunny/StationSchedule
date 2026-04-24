using CloudConvert.API.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaConverter.Models
{
    internal class ConversionJob
    {
        public required ConversionTask conversionTask {  get; set; }

        public Func<ConversionTask, CancellationToken, Task>? Handler { get; init; }

        public JobStatus Status { get; internal set; } = JobStatus.Queued;

        public Exception? Error { get; internal set; }

        public Task ExecuteAsync(CancellationToken ct)
       => Handler!(conversionTask, ct);
        
    }

    public enum JobStatus
    {
        Queued,
        Processing,
        Completed,
        Failed
    }
}
