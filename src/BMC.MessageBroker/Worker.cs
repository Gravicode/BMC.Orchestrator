using MQTTnet.Server;
using MQTTnet;
using MQTTnet.Protocol;
using BMC.MessageBroker.Data;
using Gravicode.Tools;

namespace BMC.MessageBroker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        UserProfileService UserSvc;
        DeviceService DeviceSvc;

        public Worker(ILogger<Worker> logger, UserProfileService UserProfileService, DeviceService DeviceService)
        {
            this.UserSvc = UserProfileService;
            this.DeviceSvc = DeviceService;
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
                    var valid = DeviceSvc.IsMqttClientIdExist(e.ClientId, e.UserName);
                    if (!valid.Result)
                    {
                        e.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                    }


                    var item = UserSvc.GetItemByEmail(e.UserName);
                    if (item == null)
                    {
                        e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    }
                    else
                    {
                        var enc = new Encryption();
                        var pass = enc.Decrypt(item.Password);
                        bool isAuthenticate = pass == e.Password;
                        if (!isAuthenticate)
                        {
                            e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                        }
                    }

                    _logger.LogInformation("MQTT Server is Ready: {time}", DateTimeOffset.Now);


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