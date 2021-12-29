// See https://aka.ms/new-console-template for more information
using BMC.StreamProcessor;
using Gravicode.Tools;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;

Console.WriteLine("Testing BMC Orchestrator");

Random rnd = new Random();  
var datas = new List<SensorData>();
foreach(var x in Enumerable.Range(0, 10))
{
    datas.Add(new SensorData() { Temp = rnd.Next(1,100), Distance = rnd.Next(1,10) });
}

var pipe = new StreamPipe();
pipe.OnAlertTriggered += (a, b) => { Console.WriteLine($"triggered : {b.AlertName} -> {b.DataObject?.ToString()}"); };
pipe.AddAlert("temp alert", "Temp > 30");
pipe.AddAlert("distance alert", "distance < 6");

foreach(var item in datas)
{
    pipe.ProcessData(JsonConvert.SerializeObject( item));
    Thread.Sleep(500);
}

Console.WriteLine("completed...");

ParseSQLSample sample = new ParseSQLSample();
sample.Run();

ReadConfig();
/*
//sample mqtt broker
MqttBroker broker = new MqttBroker();
broker.Start();

Console.ReadLine();

broker.Stop();
*/

Console.ReadLine();

static void ReadConfig()
{
    try
    {
        var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("config.json", optional: false);

        IConfiguration Configuration = builder.Build();

        MailService.MailUser = Configuration["MailSettings:MailUser"];
        MailService.MailPassword = Configuration["MailSettings:MailPassword"];
        MailService.MailServer = Configuration["MailSettings:MailServer"];
        MailService.MailPort = int.Parse(Configuration["MailSettings:MailPort"]);
        MailService.SetTemplate(Configuration["MailSettings:TemplatePath"]);
        MailService.SendGridKey = Configuration["MailSettings:SendGridKey"];
        MailService.UseSendGrid = true;


        SmsService.UserKey = Configuration["SmsSettings:ZenzivaUserKey"];
        SmsService.PassKey = Configuration["SmsSettings:ZenzivaPassKey"];

    }
    catch (Exception ex)
    {
        Console.WriteLine("read config failed:" + ex);
    }
    Console.WriteLine("Read config successfully.");
}
public class SensorData
{
    public float Distance { get; set; }
    public int Temp { get; set; }
}