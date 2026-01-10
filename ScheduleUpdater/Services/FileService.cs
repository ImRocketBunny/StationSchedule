using ScheduleUpdater.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleUpdater.Services
{
    internal sealed class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;   
        private readonly IConfiguration _configuration; 
        private readonly HttpClient _httpClient;
        private DateTime _runDate;
        public FileService(ILogger<FileService> logger, IConfiguration configuration) 
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
            _runDate = DateTime.Now;
        }

        public async Task FileServiceManager()
        {
            if (_runDate < DateTime.Now)
            {
                await DownloadSchedules();
                GC.Collect();
                await CopyArchive();
                GC.Collect();
                await UnzipSchedules();
                GC.Collect();
                _runDate = _runDate.AddHours(12);
                _logger.LogInformation($"Next ruin will begin at {_runDate}");
            }
          

        }

        private async Task DownloadSchedules()
        {
            var schedulesToUpdate = _configuration.GetSection("ScheduleSources").Get<List<ScheduleSource>>();
            foreach (ScheduleSource ss in schedulesToUpdate.Where(e => e.Update == true).ToList()) 
            {
                _logger.LogInformation($"Get {ss.SourceLink}");
                using (var response = await _httpClient.GetAsync(ss.SourceLink))
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    if (File.Exists($"ScheduleFiles\\ZipDownloads\\{ss.Name}.zip"))
                    {
                        _logger.LogInformation($"Deleting file {ss.Name}");
                        File.Delete($"ScheduleFiles\\ZipDownloads\\{ss.Name}.zip");
                        _logger.LogInformation($"File {ss.Name} has been deleted");
                    }
                    _logger.LogInformation("");
                    using (var file = File.OpenWrite($"ScheduleFiles\\ZipDownloads\\{ss.Name}.zip"))
                    {
                        stream.CopyTo(file);
                    }
                }
            }

                

        }


        private Task UnzipSchedules()
        {
            string[] fileEntries = Directory.GetFiles("ScheduleFiles\\UnpackedSchedules");
            foreach (string fileEntry in fileEntries) 
            {
                _logger.LogInformation($"Unziping file {fileEntry}");
                if(Directory.Exists(fileEntry.Replace(".zip", "")))
                {
                    _logger.LogInformation($"Deleting folder: {fileEntry.Replace(".zip", "")}");
                    Directory.Delete(fileEntry.Replace(".zip", ""),true);
                }
                ZipFile.ExtractToDirectory(fileEntry, fileEntry.Replace(".zip", ""),true);
            }
            return Task.CompletedTask;
        }


        private  Task CopyArchive()
        {

            string[] fileEntries = Directory.GetFiles("ScheduleFiles\\ZipDownloads");
            foreach (string fileEntry in fileEntries)
                if(File.Exists(fileEntry.Replace("ScheduleFiles\\ZipDownloads", "ScheduleFiles\\UnpackedSchedules")))
                {
                    if (checkMD5(fileEntry) != checkMD5(fileEntry.Replace("ScheduleFiles\\ZipDownloads", "ScheduleFiles\\UnpackedSchedules")))
                    {
                        File.Copy(fileEntry, fileEntry.Replace("ScheduleFiles\\ZipDownloads", "ScheduleFiles\\UnpackedSchedules"), true);
                    }
                }
                else
                {
                    File.Copy(fileEntry, fileEntry.Replace("ScheduleFiles\\ZipDownloads", "ScheduleFiles\\UnpackedSchedules"), true);

                }

            return Task.CompletedTask;
        }


        private string checkMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return Encoding.Default.GetString(md5.ComputeHash(stream));
                }
            }
        }
    }
}
