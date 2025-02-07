
using AudioAnnouncementService.Abstract;
using AudioAnnouncementService.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;
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
        private readonly ITaskManagerService _taskManagerService;
        //private List<FullCourse> _courses = new List<FullCourse>();
        public Worker(ILogger<Worker> logger, ITaskManagerService taskManagerService)
        {
            _logger = logger;
            _taskManagerService = taskManagerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            /*var factory = new MqttFactory();

            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883) 
                .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
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
      "IV/10",
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
                //await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("station/IV/8").Build());



                
                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    if (e.ApplicationMessage.Topic.ToString().Contains("delay"))
                    {
                        Console.WriteLine("Nothing");
                    }
                    //ApplicationMessageReceivedAsync
                    var waveOut = new WasapiOut();

                    FullCourse courses = JsonConvert.DeserializeObject<FullCourse>(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
                    if(courses ==null || courses.Name is null)
                    {
                        return Task.CompletedTask;
                    }
                    _courses.Add(courses);
                    string nazwa = courses.Name.Split(" ")[0];
                    List<AudioFileReader> lista = new List<AudioFileReader>();
                    //AudioFileReader[] playlista = new AudioFileReader[nazwy.Length];

                    
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
                        if (courses.HeadsignTo != "" && courses.HeadsignFrom == "" && FileList3.Contains(courses.HeadsignTo))
                        {
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\do_stacji.mp3")));
                            lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + courses.HeadsignTo + ".mp3"));
                        }
                        if (courses.HeadsignTo == "" && courses.HeadsignFrom != "" && FileList3.Contains(courses.HeadsignFrom))
                        {
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\ze_stacji.mp3")));
                            lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + courses.HeadsignFrom + ".mp3"));
                        }
                        if (courses.HeadsignTo != "" && courses.HeadsignFrom != "" && FileList3.Contains(courses.HeadsignFrom) && FileList3.Contains(courses.HeadsignTo))
                        {
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\ze_stacji.mp3")));
                            lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + courses.HeadsignFrom + ".mp3"));
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\do_stacji.mp3")));
                            lista.Add(new AudioFileReader(".\\Sounds\\Stations\\" + courses.HeadsignTo + ".mp3"));
                        }
                        //var otherStations = Regex.Split(Regex.Replace(Regex.Replace(Regex.Replace(courses.RouteTo, "[0-9][0-9]:[0-9][0-9]", ""), " •  ", " -  "), "  ", ""), " -");
                        
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
                            lista.Add(new AudioFileReader((".\\Sounds\\Platform\\" + courses.Platform.Split("/")[0]+".mp3")));

                            if (courses.DepartureTime is null)
                            {
                                lista.Add(new AudioFileReader((".\\Sounds\\Core\\pociąg_konczy.mp3")));
                            }
                            else
                            {
                                if (courses.ArrivalTime != courses.DepartureTime)
                                {
                                    lista.Add(new AudioFileReader((".\\Sounds\\Core\\planowy_odjazd.mp3")));
                                    lista.Add(new AudioFileReader((".\\Sounds\\Time\\Hours\\" + (courses.DepartureTime).Split(":")[0] + ".mp3")));
                                    lista.Add(new AudioFileReader((".\\Sounds\\Time\\Minutes\\" + (courses.DepartureTime).Split(":")[1] + ".mp3")));
                                }


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
                            lista.Add(new AudioFileReader((".\\Sounds\\Platform\\" + courses.Platform.Split("/")[0] + ".mp3")));

                            if (courses.Delay == "")
                            {
                                lista.Add(new AudioFileReader((".\\Sounds\\Core\\planowy_odjazd.mp3")));
                                lista.Add(new AudioFileReader((".\\Sounds\\Time\\Hours\\" + (courses.DepartureTime).Split(":")[0] + ".mp3")));
                                lista.Add(new AudioFileReader((".\\Sounds\\Time\\Minutes\\" + (courses.DepartureTime).Split(":")[1] + ".mp3")));
                            }
                        }



                        if(courses.ArrivalTime==null)
                        {
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\życzymy.mp3")));
                        }
                        else
                        {
                            lista.Add(new AudioFileReader((".\\Sounds\\Core\\prosimy_zachować.mp3")));

                        }


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
            }*/
            await WaitUntilStoppedAsync(stoppingToken);
        }

        private async Task WaitUntilStoppedAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                await _taskManagerService.Execute();
               // _logger.LogInformation("Worker is working");
                Thread.Sleep(1000);
            }
        }
    }
}
