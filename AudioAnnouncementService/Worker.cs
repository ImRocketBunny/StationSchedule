
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
using System.Text.RegularExpressions;
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
            DirectoryInfo d2 = new DirectoryInfo(".\\Sounds\\TrainCustomNames\\"); //Assuming Test is your Folder
            DirectoryInfo d3 = new DirectoryInfo(".\\Sounds\\Stations\\");
            FileInfo[] Files = d.GetFiles("*.mp3"); //Getting Text files
            FileInfo[] Files2 = d2.GetFiles("*.mp3");
            FileInfo[] Files3 = d3.GetFiles("*.mp3");
            List<string> FileList= Files.Select(e => e.Name.Replace(".mp3","")).ToList();
            List<string> FileList2 = Files2.Select(e => e.Name.Replace(".mp3", "")).ToList();
            List<string> FileList3 = Files3.Select(e => e.Name.Replace(".mp3", "")).ToList();


            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                Console.WriteLine("Connected to MQTT broker successfully.");

                // Subscribe to a topic
                var response = await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/main/departures").Build());

                // Callback function when a message is received
                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    var waveOut = new WasapiOut();

                    List<Course> courses = JsonConvert.DeserializeObject<List<Course>>(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));

                    string[] nazwy = courses.Select(e=>e.Name.Split(" ")[0]).ToArray();
                    List<AudioFileReader> lista = new List<AudioFileReader>();
                    //AudioFileReader[] playlista = new AudioFileReader[nazwy.Length];
                    foreach (Course c in courses)
                    {
                        Console.WriteLine((int)(TimeOnly.Parse(c.Time) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes);
                    }

                    foreach (Course c in courses.Where(c=>c.Name.Split(" ")[0] != string.Empty 
                    && FileList.Contains(c.Name.Split(" ")[0]) &&
                    (int)(TimeOnly.Parse(c.Time) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes < 400))
                    {
                        //Console.WriteLine((int)(TimeOnly.Parse(c.Time) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes);
                        lista.Add(new AudioFileReader(".\\Sounds\\Core\\"+ c.Name.Split(" ")[0] + ".mp3"));
                        if (c.Name.Split("   ").Length>1&& FileList2.Contains(c.Name.Split("   ")[1]))
                            lista.Add(new AudioFileReader(".\\Sounds\\TrainCustomNames\\" + c.Name.Split("   ")[1] + ".mp3"));
                        lista.Add(new AudioFileReader((".\\Sounds\\Core\\do_stacji.mp3")));
                        if(FileList3.Contains(c.Headsign))
                            lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + c.Headsign + ".mp3"));
                        var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(c.Route, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");
                        if(otherStations.Length > 1)
                        {
                            for(int i = 1; i < otherStations.Length - 1; i++)
                            {
                                try
                                {
                                    lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + otherStations[i] + ".mp3"));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                            
                        }
                        lista.Add(new AudioFileReader((".\\Sounds\\Core\\planowy_przyjazd.mp3")));
                        lista.Add(new AudioFileReader((".\\Sounds\\Time\\Hours\\" + c.Time.Split(":")[0] + ".mp3")));

                        try
                        {
                            lista.Add(new AudioFileReader((".\\Sounds\\Time\\Minutes\\" + c.Time.Split(":")[1] + ".mp3")));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        //lista.Add(new AudioFileReader((".\\Sounds\\Time\\Hours\\"+c.Time.Split(":")[0]+".mp3")));
                        //lista.Add(new AudioFileReader((".\\Sounds\\Time\\Minutes\\" + c.Time.Split(":")[1] + ".mp3")));
                        if (c.Delay != string.Empty)
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\za_opóŸnienie.mp3")));
                    }
                    AudioFileReader[] playlista = lista.ToArray();
                    if (playlista.Length > 0)
                    {
                        var playlist = new ConcatenatingSampleProvider(playlista);
                        waveOut.Init(playlist);
                        waveOut.Play();
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(1000);
                        }
                        waveOut.Dispose();
                    }
                    
                    Thread.Sleep(60000);
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
