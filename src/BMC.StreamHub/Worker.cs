using BMC.Models;
using BMC.StreamHub.Data;
using BMC.StreamProcessor;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace BMC.StreamHub
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private MessageProcessor _mqtt;
        private AlertProcessor _alert;
        MqttTopicService MqttTopicSvc;
        MessageStreamService MessageStreamSvc;
        AlertService AlertSvc;
        public Worker(ILogger<Worker> logger, MqttTopicService MqttTopicSvc, MessageStreamService MessageStreamSvc, AlertService AlertSvc)
        {
            this.AlertSvc = AlertSvc;
            this.MessageStreamSvc = MessageStreamSvc;
            this.MqttTopicSvc = MqttTopicSvc;
            _logger = logger;
            _alert = new AlertProcessor();
            _mqtt = new MessageProcessor(AppConstants.MqttHost, AppConstants.MqttPort, "admin", "123qweasd");
            _mqtt.MessageReceived += (a, e) =>
            {
                //save stream
                var selTopic = MqttTopicSvc.GetDataByTopic(e.Topic);
                var uname = string.Empty;
                if (selTopic != null)
                {
                    uname = selTopic.Username;
                }
                var res = this.MessageStreamSvc.InsertData(new Models.MessageStream() { Content = e.Message, CreatedDate = DateTime.Now, MqttClientId = "stream-hub", MqttTopic = e.Topic, Username = uname });
                _logger.LogInformation($"new data received and saved: {e.Topic} -> {e.Message}");
                //trigger alert
                _alert.ProcessMessage(e.Topic, e.Message);
            };

            _logger.LogInformation("Message Hub Service is ready to serve");
        }
        void UpdateTopics()
        {
            var topics = MqttTopicSvc.GetAllData();
            var seltopics = topics.Select(x => x.Topic).ToArray();
            //re-init mqtt if disconnected
            if (!_mqtt.IsReady)
            {
                _mqtt.Setup();
                _mqtt.SubscribeTopics(seltopics);
            }
            else
            {
                _mqtt.SubscribeTopics(seltopics);
            }
        }
        void UpdateAlerts()
        {
            var ListAlert = AlertSvc.GetAllData();
            _alert.Setup(ListAlert);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                UpdateTopics();
                UpdateAlerts();
                await Task.Delay(60 * 1000, stoppingToken);
            }
        }
    }
    public class IncomingMessageArgs : EventArgs
    {
        public string Message { get; set; }
        public string Topic { get; set; }
        public string ClientId { get; set; }
    }
    public class MessageProcessor
    {
        public EventHandler<IncomingMessageArgs> MessageReceived;
        public bool IsReady { get; set; } = false;
        HashSet<string> SubTopics = new();
        public string MqttHost { get; set; }
        public string MqttPort { get; set; }
        public string UserName { get; set; }
        public string Pass { get; set; }
        public string DeviceId { get; set; }
        public MessageProcessor(string host, string port, string username, string password, string deviceid = "stream-hub")
        {
            this.UserName = username;
            this.MqttPort = port;
            this.MqttHost = host;
            this.Pass = password;
            this.DeviceId = deviceid;
            Setup();

        }
        MqttClient client { set; get; }
        public void Setup()
        {
            try
            {

                // create client instance
                client = new MqttClient(this.MqttHost);

                // register to message received
                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                client.Connect(this.DeviceId, this.UserName, this.Pass);

                IsReady = true;

                Console.WriteLine("MQTT is ready");
            }
            catch (Exception ex)
            {
                IsReady = false;
                Console.WriteLine("Init mqtt error:" + ex);
            }


        }

        public void SubscribeTopics(string[] Topics)
        {
            //sub new topic
            foreach (var item in Topics)
            {
                if (!SubTopics.Contains(item))
                {
                    SubTopics.Add(item);
                    client.Subscribe(new string[] { item }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                }
            }
            //unsubscribe non exist topic
            var listUnsubscribe = new List<string>();
            foreach (var item in SubTopics)
            {
                if (!Topics.Contains(item))
                {
                    SubTopics.Remove(item);
                    listUnsubscribe.Add(item);
                }
            }
            if (listUnsubscribe.Count > 0)
                client.Unsubscribe(listUnsubscribe.ToArray());

        }
        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var Pesan = System.Text.Encoding.UTF8.GetString(e.Message);
            MessageReceived?.Invoke(this, new IncomingMessageArgs() { Message = Pesan, Topic = e.Topic });

        }
    }

    public class AlertProcessor
    {
        //1 pipe per mqtt topic
        Dictionary<string, StreamPipe> Pipes = new();
        public AlertProcessor()
        {

        }

        public void ProcessMessage(string Topic,string Message)
        {
            if (Pipes.ContainsKey(Topic))
            {
                try
                {
                    var pipe = Pipes[Topic];
                    pipe.ProcessData(Message);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error execute pipe for {Topic}: {ex}");
                }
            }
        }
        public void Setup(List<Alert> ListAlert)
        {
            var topics = ListAlert.Select(x => x.MqttTopic?.Topic).Distinct().ToList();
            //add new topic to alert
            foreach(var topic in topics)
            {
                if (Pipes.ContainsKey(topic))
                {

                }
                else
                {
                    var pipe = new StreamPipe();
                    pipe.OnAlertTriggered += (a, b) => { Console.WriteLine($"[{DateTime.Now}] alert triggered: {b.AlertName} -> {b.DataObject?.ToString()}"); };
                    var alerts = ListAlert.Where(x => x.MqttTopic?.Topic == topic);
                    foreach(var alert in alerts)
                    {
                        pipe.AddAlert(alert.Name, alert.FilterQuery);
                        if (!string.IsNullOrEmpty(alert.SendToEmail))
                        {
                            pipe.AlertActions.Add(new EmailAction(alert.Name, $"{alert.Name} triggered by {alert.FilterQuery}", alert.SendToEmail));
                        }
                        if (!string.IsNullOrEmpty(alert.SendToPhone))
                        {
                            pipe.AlertActions.Add(new SmsAction(alert.SendToPhone, $"{alert.Name} triggered by {alert.FilterQuery}"));
                        }
                        if (!string.IsNullOrEmpty(alert.CallUrl))
                        {
                            pipe.AlertActions.Add(new UrlAction(alert.CallUrl));
                        }
                    }
                    Pipes.Add(topic,pipe);
                }

            }
            //remove unused alert
            foreach(var key in Pipes.Keys)
            {
                if (!topics.Contains(key))
                {
                    Pipes.Remove(key);
                }
            }
        }
    }
}