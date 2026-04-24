using MediaConverter.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaConverter.Services
{
    internal sealed class MediaConverterService : IMediaConversionService
    {
        private readonly IMediaConversionQueueService _mediaConversionService;
        private readonly ILogger<MediaConverterService> _logger;
        private readonly IConfiguration _configuration;
        public MediaConverterService(IMediaConversionQueueService mediaConversionQueueService, ILogger<MediaConverterService> logger, IConfiguration configuration) 
        {
            _mediaConversionService = mediaConversionQueueService;
            _logger = logger;
            _configuration = configuration;

        }



        public async Task RunMediaConverter()
        {

        }


    }
}
