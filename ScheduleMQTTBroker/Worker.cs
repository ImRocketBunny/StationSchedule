using MQTTnet;
using MQTTnet.Server;
using System.Text;
namespace ScheduleMQTTBroker
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
            _logger.LogInformation("Schedule MQTT Broker service is starting...");
            await WaitUntilStoppedAsync(stoppingToken);
        }

        private async Task WaitUntilStoppedAsync(CancellationToken stoppingToken)
        {
            /*while (!stoppingToken.IsCancellationRequested)
            {*/
            var option = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883);
                /*.WithConnectionValidator(c =>
                {
                    if (c.ClientId.Length < 10)
                    {
                        c.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                        return;
                    }

                    if (c.Username != "mySecretUser")
                    {
                        c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                        return;
                    }

                    if (c.Password != "mySecretPassword")
                    {
                        c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                        return;
                    }

                    c.ReasonCode = MqttConnectReasonCode.Success;
                })
                .WithApplicationMessageInterceptor(context =>
                {
                    if (context.ApplicationMessage.Topic == "my/custom/topic")
                    {
                        context.ApplicationMessage.Payload = Encoding.UTF8.GetBytes("The server injected payload.");
                    }

                    // It is possible to disallow the sending of messages for a certain client id like this:
                    if (context.ClientId != "Someone")
                    {
                        context.AcceptPublish = false;
                        return;
                    }
                    // It is also possible to read the payload and extend it. For example by adding a timestamp in a JSON document.
                    // This is useful when the IoT device has no own clock and the creation time of the message might be important.
                });*/
            //mqttServer.InterceptingPublishAsync += Server_InterceptingPublishAsync;
            var mqttServer = new MqttFactory().CreateMqttServer(option.Build());
            mqttServer.InterceptingPublishAsync += Server_InterceptingPublishAsync;
            mqttServer.ValidatingConnectionAsync += Server_ValidatingConnectionAsync;
            mqttServer.InterceptingSubscriptionAsync += Server_InterceptingSubscription;
            await mqttServer.StartAsync();
                await Task.Delay(1000);
            //}

        }
        Task Server_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            // Convert Payload to string
            var payload = arg.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(arg.ApplicationMessage?.Payload);


            _logger.LogInformation(
                " TimeStamp: {0} -- Message: ClientId = {1}, Topic = {2}, QoS = {3}, Retain-Flag = {4}",

                DateTime.Now,
                arg.ClientId,
                arg.ApplicationMessage?.Topic,
                //payload,
                arg.ApplicationMessage?.QualityOfServiceLevel,
                arg.ApplicationMessage?.Retain);
            return Task.CompletedTask;
        }

        Task Server_ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
        {
            // Convert Payload to string
            var payload = arg.ClientId;
            _logger.LogInformation(arg.ClientId, arg.Password);
            //validate
            /*Console.WriteLine(
                " TimeStamp: {0} -- Message: ClientId = {1}, Topic = {2}, Payload = {3}, QoS = {4}, Retain-Flag = {5}",

                DateTime.Now,
                arg.ClientId,
                arg.ApplicationMessage?.Topic,
                payload,
                arg.ApplicationMessage?.QualityOfServiceLevel,
                arg.ApplicationMessage?.Retain);*/
            return Task.CompletedTask;
        }

        Task Server_InterceptingSubscription(InterceptingSubscriptionEventArgs arg)
        {




            _logger.LogInformation(
                " TimeStamp: {0} -- Sunscription: ClientId = {1}, Topic = {2}",

                DateTime.Now,
                arg.ClientId,
                arg.TopicFilter.Topic);
            
            return Task.CompletedTask;
        }
        /*public static void OnNewMessage(MqttApplicationMessageInterceptorContext context)
        {
            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);


            Log.Logger.Information(
                "MessageId: {MessageCounter} - TimeStamp: {TimeStamp} -- Message: ClientId = {clientId}, Topic = {topic}, Payload = {payload}, QoS = {qos}, Retain-Flag = {retainFlag}",
                MessageCounter,
                DateTime.Now,
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain);
        }

        public static void OnNewConnection(MqttConnectionValidatorContext context)
        {
            Log.Logger.Information(
                    "New connection: ClientId = {clientId}, Endpoint = {endpoint}, CleanSession = {cleanSession}",
                    context.ClientId,
                    context.Endpoint,
                    context.CleanSession);
        }*/
    }
}
