using MQTTnet.Server;
using MQTTnet;
using MQTTnet.Protocol;

namespace BMC.MessageBroker
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
            if (!IsRunning) await StartBroker();
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
        public bool IsRunning { get; set; }

        public void StopBroker()
        {
            IsRunning = false;
        }
        async Task StartBroker()
        {

            var mqttFactory = new MqttFactory();

            var mqttServerOptions = new MqttServerOptionsBuilder().WithDefaultEndpoint().Build();

            using (var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions))
            {
                // Setup connection validation before starting the server so that there is 
                // no change to connect without valid credentials.
                mqttServer.ValidatingConnectionAsync += e =>
                {
                    if (e.ClientId != "ValidClientId")
                    {
                        e.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                    }

                    if (e.UserName != "ValidUser")
                    {
                        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    }

                    if (e.Password != "SecretPassword")
                    {
                        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    }

                    return Task.CompletedTask;
                };

                await mqttServer.StartAsync();
                IsRunning = true;
                while (IsRunning)
                {
                    Thread.Sleep(1000);
                }

                await mqttServer.StopAsync();
            }
        }
    }
}