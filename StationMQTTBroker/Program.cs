using MQTTnet.AspNetCore;
using StationMQTTBroker;
using StationMQTTBroker.Abstract;
using StationMQTTBroker.Services;


var builder = WebApplication.CreateBuilder(args);

var mqttBrokerOptions = builder.Configuration.GetSection("MqttBroker");
var portTcp = mqttBrokerOptions.GetValue("PortTCP", 1883);
var portWs = mqttBrokerOptions.GetValue("PortWS", 1884);


builder.WebHost.ConfigureKestrel(option =>
{
    option.ListenAnyIP(portTcp, l => l.UseMqtt());
    option.ListenAnyIP(portWs/*, l=> l.UseHttps()*/);
});

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IMqttBrokerService, MqttBrokerService>();
builder.Services.AddMqttConnectionHandler();
builder.Services.AddConnections();
builder.Services.AddHostedMqttServer(
    optionsBuilder => { optionsBuilder.WithDefaultEndpoint(); }
);
var app = builder.Build();
app.UseRouting();

app.MapConnectionHandler<MqttConnectionHandler>(
    "/mqtt",
    httpConnectionDispatcherOptions => httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector =
        protocolList => protocolList.FirstOrDefault() ?? string.Empty);
var mqttBrokerService = app.Services.GetRequiredService<IMqttBrokerService>();
app.UseMqttServer(
    server =>
    {
        mqttBrokerService.AddMqttHandlers(server);
        //mqttBrokerService.LoadMqttStructureAsync(server);
    });

app.Run();
