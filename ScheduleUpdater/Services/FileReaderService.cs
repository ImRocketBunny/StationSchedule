using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleUpdater.Services
{
    internal class FileReaderService : IFileReaderService
    {
        public FileReaderService() { }

        public async Task FileReaderManager(string file)
        {
            //GetFileHeaders(file);
            await GetJsonFileContent(file);
        }
       /* private string[] GetFileHeaders(string fileName) 
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                string line;
                if((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    line = sr.ReadLine()!;
                    return line.Split(",");
                }

                return new string[0];
                
            }

        }*/

        private async Task<string> GetJsonFileContent(string fileName)
        {
            var fileObjects= new List<string>();
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<dynamic>();
                string json = JsonConvert.SerializeObject(records, Formatting.Indented);
                Console.WriteLine(json);
                return json;
            }
            

        }

       
    }
}
