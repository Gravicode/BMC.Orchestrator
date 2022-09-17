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
        MqttServer server;
        public bool IsRunning { get; set; }

        public Worker(ILogger<Worker> logger, UserProfileService UserProfileService, DeviceService DeviceService)
        {
            this.UserSvc = UserProfileService;
            this.DeviceSvc = DeviceService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!IsRunning) 
                this.server = await StartBroker();
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Service running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }

        public async Task StopBroker()
        {
            if (IsRunning && server!=null)
            {
                IsRunning = false;
                await this.server.StopAsync();
            }
        }
        async Task<MqttServer> StartBroker(bool enableLogger=false)
        {
            if (IsRunning) return this.server;
            var mqttFactory = enableLogger ? new MqttFactory(new ConsoleLogger()) : new MqttFactory();

            var mqttServerOptions = new MqttServerOptionsBuilder().WithDefaultEndpoint().Build();

            var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);
            {
                // Setup connection validation before starting the server so that there is 
                // no change to connect without valid credentials.
                mqttServer.ValidatingConnectionAsync += e =>
                {
                    if (e.ClientId != "stream-hub")
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

                    }


                    return Task.CompletedTask;
                };

                await mqttServer.StartAsync();
                _logger.LogInformation("MQTT Server is Ready: {time}", DateTimeOffset.Now);
                IsRunning = true;
                return mqttServer;
            }
        }
    }
}