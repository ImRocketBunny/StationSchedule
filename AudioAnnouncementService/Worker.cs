
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
            List<string> FileList = Files.Select(e => e.Name.Replace(".mp3", "")).ToList();
            List<string> FileList2 = Files2.Select(e => e.Name.Replace(".mp3", "")).ToList();
            List<string> FileList3 = Files3.Select(e => e.Name.Replace(".mp3", "")).ToList();


            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                Console.WriteLine("Connected to MQTT broker successfully.");

                // Subscribe to a topic
                string[] platformTrack = [
      "WKD",
      "II/22",
      "II/20",
      "III/21",
      "III/23",
      "IV/8",
      "V/6",
      "V/4",
      "VI/4",
      "VI/2",
      "VII/3",
      "VII/5",
      "VIII/7",
      "IX/1",
      "IX/2",
      "BUS"
    ];
                foreach (string platform in platformTrack)
                {
                    await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/"+platform+"/audio").Build());
                }
               // await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/VI/2").Build());
                //await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/IX/1").Build());
                //await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/WKD").Build());
                //await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/II/22").Build());
                //await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/II/20").Build());
                //await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/III/21").Build());
                //await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/III/23").Build());
                //await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/IV/8").Build());*/



                // Callback function when a message is received
                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    //ApplicationMessageReceivedAsync
                    var waveOut = new WasapiOut();

                    FullCourse courses = JsonConvert.DeserializeObject<FullCourse>(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
                    if(courses ==null || courses.Name is null)
                    {
                        return Task.CompletedTask;
                    }
                    string nazwa = courses.Name.Split(" ")[0];
                    List<AudioFileReader> lista = new List<AudioFileReader>();
                    //AudioFileReader[] playlista = new AudioFileReader[nazwy.Length];
                    /*foreach (Course c in courses)
                    {
                        Console.WriteLine((int)(TimeOnly.Parse(c.Time) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes);
                    }*/
                    
                   // courses.OrderBy(c => c.ArrivalTime ?? c.DepartureTime);
                    if (  courses.Name.Split(" ")[0] != string.Empty
                    && FileList.Contains(courses.Name.Split(" ")[0]) &&
                    (int)(TimeOnly.Parse(courses.ArrivalTime ?? courses.DepartureTime) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes < 10
                    && (int)(TimeOnly.Parse(courses.ArrivalTime ?? courses.DepartureTime) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes >=1)
                    {
                        //Console.WriteLine((int)(TimeOnly.Parse(c.Time) - TimeOnly.Parse(DateTime.Now.ToString("HH:mm"))).TotalMinutes);
                        lista.Add(new AudioFileReader(".\\Sounds\\Core\\" + courses.Name.Split(" ")[0] + ".mp3"));
                        if (courses.Name.Split("   ").Length > 1 && FileList2.Contains(courses.Name.Split("   ")[1].Split("/")[0]))
                            lista.Add(new AudioFileReader(@".\\Sounds\\TrainCustomNames\\" + courses.Name.Split("   ")[1].Split("/")[0] + ".mp3"));
                        //lista.Add(new AudioFileReader((".\\Sounds\\Core\\do_stacji.mp3")));
                        if (courses.HeadsignTo != "" && FileList3.Contains(courses.HeadsignTo))
                        {
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\do_stacji.mp3")));
                            lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + courses.HeadsignTo + ".mp3"));
                        }
                        if (courses.HeadsignTo == "" && courses.HeadsignFrom != "" && FileList3.Contains(courses.HeadsignFrom))
                        {
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\ze_stacji.mp3")));
                            lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + courses.HeadsignFrom + ".mp3"));
                        }
                        //var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(courses.RouteTo, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");
                        /*if (otherStations.Length > 1)
                        {
                            lista.Add(new AudioFileReader(".\\Sounds\\Core\\przez_stacje.mp3"));
                            for (int i = 1; i < otherStations.Length-1; i++)
                            {
                                try
                                {
                                    lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + otherStations[i] + ".mp3"));
                                    Console.Write(otherStations[i] + ", ");
                                }
                                catch (Exception ex)
                                {
                                    Console.Write(otherStations[i] + ", ");
                                    //Console.WriteLine(ex.ToString());
                                }
                            }

                        }*/
                        if (courses.ArrivalTime is not null)
                        {
                            if (courses.DepartureTime is not null)
                            {
                                var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(courses.RouteTo, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");
                                if (otherStations.Length > 2)
                                {
                                    lista.Add(new AudioFileReader(".\\Sounds\\Core\\przez_stacje.mp3"));
                                    for (int i = 1; i < otherStations.Length - 1; i++)
                                    {
                                        try
                                        {
                                            lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + otherStations[i] + ".mp3"));
                                            Console.Write(otherStations[i] + ", ");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.Write(otherStations[i] + ", ");
                                            //Console.WriteLine(ex.ToString());
                                        }
                                    }

                                }
                            }

                            if (courses.DepartureTime is null)
                            {
                                var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(courses.RouteFrom, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");
                                if (otherStations.Length > 2)
                                {
                                    lista.Add(new AudioFileReader(".\\Sounds\\Core\\przez_stacje.mp3"));
                                    for (int i = 1; i < otherStations.Length - 1; i++)
                                    {
                                        try
                                        {
                                            lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + otherStations[i] + ".mp3"));
                                            Console.Write(otherStations[i] + ", ");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.Write(otherStations[i] + ", ");
                                            //Console.WriteLine(ex.ToString());
                                        }
                                    }

                                }
                            }

                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\planowy_przyjazd.mp3")));
                            lista.Add(new AudioFileReader((".\\Sounds\\Time\\Hours\\" + (courses.ArrivalTime).Split(":")[0] + ".mp3")));
                            lista.Add(new AudioFileReader((".\\Sounds\\Time\\Minutes\\" + (courses.ArrivalTime).Split(":")[1] + ".mp3")));
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\wjedzie_na.mp3")));
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\przy_peronie.mp3")));

                            if(courses.DepartureTime is null)
                            {
                                lista.Add(new AudioFileReader((".\\Sounds\\Core\\pociąg_konczy.mp3")));
                            }
                        }
                        if (courses.ArrivalTime is null && courses.DepartureTime is not null)
                        {
                            var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(courses.RouteTo, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");

                            if (otherStations.Length > 2)
                            {
                                lista.Add(new AudioFileReader(".\\Sounds\\Core\\przez_stacje.mp3"));
                                for (int i = 1; i < otherStations.Length - 1; i++)
                                {
                                    try
                                    {
                                        lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + otherStations[i] + ".mp3"));
                                        Console.Write(otherStations[i] + ", ");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.Write(otherStations[i] + ", ");
                                        //Console.WriteLine(ex.ToString());
                                    }
                                }

                            }
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\stoi_na.mp3")));
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\przy_peronie.mp3")));
                            if (courses.Delay == "")
                            {
                                lista.Add(new AudioFileReader((".\\Sounds\\Core\\planowy_odjazd.mp3")));
                                lista.Add(new AudioFileReader((".\\Sounds\\Time\\Hours\\" + (courses.DepartureTime).Split(":")[0] + ".mp3")));
                                lista.Add(new AudioFileReader((".\\Sounds\\Time\\Minutes\\" + (courses.DepartureTime).Split(":")[1] + ".mp3")));
                            }
                        }

                        /*try
                        {
                            lista.Add(new AudioFileReader((".\\Sounds\\Time\\Minutes\\" + (c.ArrivalTime ?? c.DepartureTime).Split(":")[1] + ".mp3")));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }*/
                        //lista.Add(new AudioFileReader((".\\Sounds\\Time\\Hours\\"+c.Time.Split(":")[0]+".mp3")));
                        //lista.Add(new AudioFileReader((".\\Sounds\\Time\\Minutes\\" + c.Time.Split(":")[1] + ".mp3")));
                        lista.Add(new AudioFileReader((".\\Sounds\\Core\\prosimy_zachować.mp3")));
                        if (courses.Delay != string.Empty)
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\za_opóźnienie.mp3")));
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

                    //Thread.Sleep(60000);
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
