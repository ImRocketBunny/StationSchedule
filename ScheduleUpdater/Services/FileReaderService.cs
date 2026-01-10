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
        private readonly ILogger<FileReaderService> _logger;
        private DateTime _runDate;
        public FileReaderService(IUpdaterRepository updaterRepository,ILogger<FileReaderService>logger) 
        {
            _updaterRepository = updaterRepository;
            _logger = logger;
            _runDate = DateTime.Now;
        }

        public async Task FileReaderManager()
        {
            //Console.WriteLine($"{_runDate} {DateTime.Now}");
            if (_runDate < DateTime.Now)
            {
                await GetScheduleFolders("ScheduleFiles\\UnpackedSchedules");
                await ReadScheduleFiles();
                _runDate=_runDate.AddHours(3);
                _logger.LogInformation($"Next update will begin at: {_runDate}");

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
               // if (folder.Contains("SKMW"))
               // {
                    var scheduleFiles = Directory.GetFiles(folder);
                    foreach (string file in scheduleFiles)
                    {
                        Console.WriteLine(file + " is procesing...");
                        JObject o = JObject.FromObject(new
                        {
                            scheduleName = folder.Split("\\").Last(),
                            scheduleFile = file.Split("\\").Last().Replace(".txt", ""),
                            fileContent = await GetJsonFileContent(file)
                        });
                        await _updaterRepository.UpdateSchedule(JsonConvert.SerializeObject(o, Formatting.Indented));
                    }
               // }
                
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
                return Task.FromResult(records);
            }
            

        }

       
    }
}
