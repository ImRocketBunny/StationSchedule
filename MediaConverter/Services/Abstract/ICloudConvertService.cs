using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaConverter.Services.Abstract
{
    internal interface ICloudConvertService
    {
        public Task ImportUploadFile(string inputFormat, string outputFormat, string fileName, Dictionary<string, object> options);
    }
}
