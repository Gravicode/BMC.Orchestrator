// See https://aka.ms/new-console-template for more information
using System.Net;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;
using System.Text;
using System.Text.Json;

//var topic = "mifmasterz@yahoo.com/project-a/control";
var topic = "mifmasterz@yahoo.com/project-a/data";
//var deviceid = "device-project-a-62691";
var deviceid = "device-project-a-32311";
var MQTT_BROKER_ADDRESS = "localhost";
//var MQTT_BROKER_ADDRESS = "103.250.10.88";//"cloud-iot.my.id";//
var uname = "mifmasterz@yahoo.com";
var pass = "123qweasd";
Console.WriteLine("Test mqtt client!");
// create client instance
MqttClient client = new MqttClient(MQTT_BROKER_ADDRESS);

// register to message received
client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

client.Connect(deviceid, uname,pass);

// subscribe to the topic "/home/temperature" with QoS 2
client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });



static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
{
    Console.WriteLine(e.Topic + ":" + System.Text.Encoding.UTF8.GetString(e.Message));
    // handle message received
}
var rnd = new Random();
var count = 0;
while (true)
{
    var sensor = new SensorData() { Distance = rnd.Next(100), Temp = rnd.Next(55) };
    var msg = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(sensor));

    client.Publish(topic, msg);
    Thread.Sleep(500);
}

Console.ReadLine();

public class SensorData
{
    public float Distance { get; set; }
    public int Temp { get; set; }
}