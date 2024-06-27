
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Collections.Generic;

namespace AudioAnnouncementService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {




            var reader = new AudioFileReader(".\\Sounds\\Core\\ttsMP3.com_VoiceText_2024-5-20_1-33-57.mp3");
            var waveOut = new  WasapiOut();

            var audio = new AudioFileReader(".\\Sounds\\Core\\ttsMP3.com_VoiceText_2024-5-20_1-33-57.mp3");

            var file = new AudioFileReader(".\\Sounds\\Core\\ttsMP3.com_VoiceText_2024-5-20_1-34-17.mp3");
            //waveOut.Init(reader);
            //waveOut.Play();
            var playlist = new ConcatenatingSampleProvider(new[] { reader, audio, file});
            waveOut.Init(playlist);
            waveOut.Play();
            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(1000);
            }
            //var reader2 = new Mp3FileReader("D:\\Pobrane\\Obecny odg³os zapowiedzi  gong PKP Kraków Wroc³aw Poznañ Gliwice.mp3");
            // var time = new AudioFileReader(".\\Sounds\\Core\\ttsMP3.com_VoiceText_2024-5-20_1-33-57.mp3").TotalTime;
            //Thread.Sleep(time);
            waveOut.Dispose();
            //waveOut.Init(reader2);
            //waveOut.Play();
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
