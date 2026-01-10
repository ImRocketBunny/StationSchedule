using CloudConvert.API;
using CloudConvert.API.Models;
using CloudConvert.API.Models.ExportOperations;
using CloudConvert.API.Models.ImportOperations;
using CloudConvert.API.Models.JobModels;
using CloudConvert.API.Models.TaskModels;
using CloudConvert.API.Models.TaskOperations;
using MediaConverter.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MediaConverter.Services
{
    internal sealed class CloudConvertService : ICloudConvertService

    {
        private readonly ILogger<CloudConvertService> _logger;
        private readonly IConfiguration _configuration;
        private CloudConvertAPI _cloudConvertAPI;
        public required string _filePath;
        private int _width;
        private int _height;
        private int _taskCount = 0;

        public CloudConvertService( ILogger<CloudConvertService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _cloudConvertAPI = new CloudConvertAPI(_configuration["API:API_KEY"]);
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            _filePath = _configuration["Files:InputPath"]!;

        }



        public async Task ProcessVideoConversion(string fileName)
        {
            /*var newUploadTask = await CreateUploadFileTask();
            await UploadFile(fileName, newUploadTask);
            var convertTask = await CreateConvertTask(null,null,null);
            while (convertTask.Da) {*/

        }


        public async Task ImportUploadFile(string inputFormat, string outputFormat, string fileName, Dictionary<string, object> options)
        {
            try
            {
                var job = await _cloudConvertAPI.CreateJobAsync(new JobCreateRequest
            {
                Tasks = new
                {
                    import_example_1 = new ImportUploadCreateRequest
                    {

                    },
                    convert = new ConvertCreateRequest
                    {
                        Input = "import_example_1",
                        Input_Format = inputFormat,
                        Output_Format = outputFormat,
                        Options = options is null ? new Dictionary<string, object> {
                        { "width", 1920 },
                        { "height", 1080 },
                        { "fit", "max" }
                         } : options
                    },
                    export = new ExportUrlCreateRequest
                    {
                        Input = "convert",
                        Archive_Multiple_Files = true
                    }
                },
                Tag = "Conversion"
            });
           
                Console.WriteLine($"Job created");
                var uploadTask = job.Data.Tasks.FirstOrDefault(t => t.Name == "import_example_1");
                Console.WriteLine($"UploadingFile");
                await UploadFile(fileName, uploadTask!);
                Console.WriteLine($"FileUploaded");
                Console.WriteLine($"Downloading");
                await DownloadFile(job);
                Console.WriteLine($"Downloaded");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            
            }

           

        }

        private async Task<TaskResponse> CreateUploadFileTask()
        {
            var job = await _cloudConvertAPI.CreateJobAsync(new JobCreateRequest
            {
                Tasks = new
                {
                    upload_my_file = new ImportUploadCreateRequest()
                }
            });

            var uploadTask = job.Data.Tasks.FirstOrDefault(t => t.Name == "upload_my_file");
            return uploadTask!;
        }


        private async Task UploadFile(string fileName, TaskResponse uploadTask)
        {
            using (System.IO.Stream stream = File.Open("\\Code\\MyCode\\StationSchedule\\MediaConverter\\"+fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await _cloudConvertAPI.UploadAsync(uploadTask.Result.Form.Url.ToString(), stream, fileName, uploadTask.Result.Form.Parameters);
            }
        }


        private async Task<ConvertCreateRequest> CreateConvertTask(string input_format, string output_format, Dictionary<string, object> options)
        {
            if(options is null)
            {
                options = new Dictionary<string, object> {
                    { "width", 1920 },
                    { "height", 1080 },
                    { "fit", "max" }
                };
            }

            var task = new ConvertCreateRequest
            {
                Input = "upload_my_file",
                Input_Format = "mp4",
                Output_Format = "webm",
                Options = options
            };
            return task;
        }


        private async Task DownloadFile(Response<JobResponse> job)
        {
            try
            {
                var jobRes = await _cloudConvertAPI.WaitJobAsync(job.Data.Id); // Wait for job completion
                while (jobRes.Data.Status != "finished")
                {
                    Console.WriteLine($"Waiting 4 job to finish");
                    await Task.Delay(1000);
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           

            var exportTask = job.Data.Tasks.FirstOrDefault(t => t.Name == "export");

            var fileExport = exportTask.Result.Files.FirstOrDefault();

            using (var client = new WebClient()) client.DownloadFile(fileExport.Url, fileExport.Filename);
        }


    }
}
