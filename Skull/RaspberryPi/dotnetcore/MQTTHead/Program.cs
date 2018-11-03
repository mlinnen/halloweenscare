using System;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace mqtthead
{
    class Program
    {
        static void Main(string[] args)
        {
            string deviceId = "1";

            Console.WriteLine("Hello Internet of Things!");

            MqttClient client = new MqttClient("192.168.0.25");
            
            // register to message received 
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
 
            string clientId = Guid.NewGuid().ToString(); 
            Console.WriteLine("Connect");
            client.Connect(clientId); 
 
            // subscribe to the topic "/halloween/skull/ping" with QoS 2 
            Console.WriteLine("Subscribe");
            client.Subscribe(new string[] { "/halloween/skull/ping" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            client.Subscribe(new string[] { "/halloween/skull/heady/" + deviceId }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            client.Subscribe(new string[] { "/halloween/skull/headx/" + deviceId }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            client.Subscribe(new string[] { "/halloween/skull/jaw/" + deviceId }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            client.Subscribe(new string[] { "/halloween/skull/laugh/" + deviceId }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            client.Subscribe(new string[] { "/halloween/skull/bow/" + deviceId }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
 
            Console.WriteLine("Done");
        }
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
        { 
            Console.WriteLine("Received: topic: {0} message: {1}", e.Topic,System.Text.Encoding.Default.GetString(e.Message));
        } 
    }
}
