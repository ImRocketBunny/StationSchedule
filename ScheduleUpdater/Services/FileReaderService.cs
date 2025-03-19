using CsvHelper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScheduleUpdater.Abstract;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ScheduleUpdater.Services
{
    internal class FileReaderService : IFileReaderService
    {
        private string[] _sourceFolders;
        private readonly IUpdaterRepository _updaterRepository;
        private DateTime _runDate;
        public FileReaderService(IUpdaterRepository updaterRepository) 
        {
            _updaterRepository = updaterRepository;
            _runDate = DateTime.Now;
        }

        public async Task FileReaderManager()
        {
            //Console.WriteLine($"{_runDate} {DateTime.Now}");
            if (_runDate < DateTime.Now)
            {
                await GetScheduleFolders("ScheduleFiles\\UnpackedSchedules");
                await ReadScheduleFiles();
                _runDate=_runDate.AddMinutes(3);
            }
            
            
        }
        private Task GetScheduleFolders(string path) 
        {

            _sourceFolders= Directory.GetDirectories(path);
            return Task.CompletedTask;

        }

        private async Task ReadScheduleFiles()
        {
            foreach(string folder in _sourceFolders)
            {
                var scheduleFiles = Directory.GetFiles(folder);
                foreach(string file in scheduleFiles)
                {
                    JObject o = JObject.FromObject(new
                    {
                        scheduleName = folder.Split("\\"/*LineSeparator*/).Last(),
                        scheduleFile = file.Split("\\"/*LineSeparator*/).Last().Replace(".txt", ""),
                        fileContent = await GetJsonFileContent(file)
                    });
                    await _updaterRepository.UpdateSchedule(JsonConvert.SerializeObject(o, Formatting.Indented));
                }
                await Task.Delay(100);
            }
        }

        private  Task<List<object>> GetJsonFileContent(string fileName)
        {
            var fileObjects= new List<string>();
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<object>().ToList();
                //string json = JsonConvert.SerializeObject(records, Formatting.Indented);

                return Task.FromResult(records);
            }
            

        }

       
    }
}
