using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaConverter.Services.Abstract
{
    internal interface IFFmpegConversionService
    {
        public Task ProcessFileConversion(string inputPath);
    }
}
