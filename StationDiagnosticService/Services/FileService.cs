
using StationDiagnosticService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace StationDiagnosticService.Services
{
    internal class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly IConfiguration _configuration;
        private readonly Channel<DataFrame> _channel = Channel.CreateUnbounded<DataFrame>();
        private readonly JsonSerializerOptions _jsonOptions;

        private string _basePath;
        private int _maxEntriesPerFile;
        private bool _inLineFileContent;
        private bool _singleFrameFile;
        private string _fileExtension;
        private bool _processingStarted = false;
        private Guid _guid;


        private int _currentCount = 0;
        private int _fileIndex = 0;
        private bool _isFirstInFile = true;
        


        public FileService(ILogger<FileService> logger, IConfiguration configuration) 
        {
            _logger = logger;
            _configuration = configuration;
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            _guid = Guid.NewGuid();
        }


        public async Task StartProcesingFrames()
        {
            if (!_processingStarted)
            {
                SetupFileWriter();
                _ = Task.Run(() =>
                {
                    ProcessQueueAsync();
                });
            }
            

        }

        private void SetupFileWriter()
        {

            _maxEntriesPerFile = int.Parse(_configuration["FileService:MaxFramesPerFile"]!);
            _basePath = _configuration["FileService:FileFolder"]!;
            _inLineFileContent = bool.Parse(_configuration["FileService:InLineFileContent"]!);
            _singleFrameFile = bool.Parse(_configuration["FileService:SingleFrameFile"]!);
            _fileExtension = _configuration["FileService:FileExtension"]!;
        }

        public async Task LogAsync(DataFrame dataFrame)
        {
            await _channel.Writer.WriteAsync(dataFrame);
            _logger.LogInformation($"Data frame added to file save");
        }

        private async Task ProcessQueueAsync()
        {
            _processingStarted = true;
            string currentFile = $"{_guid}_{_fileIndex}{_fileExtension}";
            _logger.LogInformation($"Created new file: {currentFile}"); ;
            await foreach (var content in _channel.Reader.ReadAllAsync())
            {
                

                if (_currentCount >= _maxEntriesPerFile)
                {
                    _logger.LogInformation($"File capacity of {_maxEntriesPerFile} has been reached, creating new file");
                    _fileIndex++;
                    _currentCount = 0;
                    _isFirstInFile = true;

                    currentFile = $"{_guid}_{_fileIndex}{_fileExtension}";
                    _logger.LogInformation($"Created new file: {currentFile}");

                }
                if (!FileExists(_basePath + Path.AltDirectorySeparatorChar + currentFile))
                {
                    FolderExists(_basePath + Path.AltDirectorySeparatorChar);
                    CreateFile(_basePath + Path.AltDirectorySeparatorChar + currentFile);
                }
                string json = JsonSerializer.Serialize(content, _jsonOptions);
                if (!_isFirstInFile)
                {
                    try
                    {
                        await File.AppendAllTextAsync(_basePath + Path.AltDirectorySeparatorChar + currentFile, ",\n");

                    }catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                }

                try
                {
                    await File.AppendAllTextAsync(_basePath + Path.AltDirectorySeparatorChar + currentFile, json);
                }catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            _logger.LogInformation($"Data saved to file.");
                _isFirstInFile = false;
                _currentCount++;

            }

            
        }

        private bool FileExists(string folderPath)
        {
            if (!File.Exists(folderPath))
            {
                return false;
            }
            return true;
        }


        private void CreateFile(string folderPath)
        {
            
            File.Create(folderPath).Dispose();
        }


        private void FolderExists(string folderPath)
        {
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

    }
}
