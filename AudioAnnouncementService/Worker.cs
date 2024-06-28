
using AudioAnnouncementService.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using Windows.UI.Xaml;
using static System.Collections.Specialized.BitVector32;

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

            var factory = new MqttFactory();

            // Create a MQTT client instance
            var mqttClient = factory.CreateMqttClient();

            // Create MQTT client options
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883) // MQTT broker address and port
                .WithCleanSession()
                .Build();


            var connectResult = await mqttClient.ConnectAsync(options);

            var msg = String.Empty;

            DirectoryInfo d = new DirectoryInfo(".\\Sounds\\Core\\"); //Assuming Test is your Folder

            FileInfo[] Files = d.GetFiles("*.mp3"); //Getting Text files
            List<string> FileList= Files.Select(e => e.Name.Replace(".mp3","")).ToList();


            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                Console.WriteLine("Connected to MQTT broker successfully.");

                // Subscribe to a topic
                var response = await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/main/dep").Build());

                // Callback function when a message is received
                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    var waveOut = new WasapiOut();

                    List<Course> courses = JsonConvert.DeserializeObject<List<Course>>(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));

                    string[] nazwy = courses.Select(e=>e.Name.Split(" ")[0]).ToArray();
                    List<AudioFileReader> lista = new List<AudioFileReader>(); 
                    //AudioFileReader[] playlista = new AudioFileReader[nazwy.Length];
                    foreach (var course in nazwy.Where(c=>c!=string.Empty && FileList.Contains(c)))
                    {
                        lista.Add(new AudioFileReader(".\\Sounds\\Core\\"+course+".mp3"));
                    }
                    AudioFileReader[] playlista = lista.ToArray();
                    var playlist = new ConcatenatingSampleProvider(playlista);
                    waveOut.Init(playlist);
                    waveOut.Play();
                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(1000);
                    }
                    waveOut.Dispose();
                    return Task.CompletedTask;
                };
            }
            //Console.WriteLine(msg);
            //var reader = new AudioFileReader(".\\Sounds\\Core\\ttsMP3.com_VoiceText_2024-5-20_1-33-57.mp3");
            //var waveOut = new  WasapiOut();

           // var audio = new AudioFileReader(".\\Sounds\\Core\\ttsMP3.com_VoiceText_2024-5-20_1-33-57.mp3");

            //var file = new AudioFileReader(".\\Sounds\\Core\\ttsMP3.com_VoiceText_2024-5-20_1-34-17.mp3");
            //waveOut.Init(reader);
            //waveOut.Play();
            //var playlist = new ConcatenatingSampleProvider(new[] { reader, audio, file});
            //waveOut.Init(playlist);
            //waveOut.Play();
            /*while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(1000);
            }*/
            //List<Course> courses = (List<Course>)JsonConvert.DeserializeObject(msg);
            /*foreach(Course c in courses)
            {
                Console.WriteLine(c.Headsign);
            }*/
            //var reader2 = new Mp3FileReader("D:\\Pobrane\\Obecny odg³os zapowiedzi  gong PKP Kraków Wroc³aw Poznañ Gliwice.mp3");
            // var time = new AudioFileReader(".\\Sounds\\Core\\ttsMP3.com_VoiceText_2024-5-20_1-33-57.mp3").TotalTime;
            //Thread.Sleep(time);
            //waveOut.Dispose();
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
