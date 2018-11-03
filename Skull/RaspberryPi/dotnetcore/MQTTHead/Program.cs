using System;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace mqtthead
{
    class Program
    {
        static MqttClient client;
        static string deviceId;
        static string baseTopic;

        static void Main(string[] args)
        {

            // Allow for the following to be passed into the command line
            // Broker Ip address
            // Broker User Name and password
            // The base topic that should be used for subscribing and publishing.
            // The device Id
            // 
            deviceId = "1";
            baseTopic = "/halloween/";

            client = new MqttClient("192.168.0.25");
            
            // register to message received 
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
 
            string clientId = Guid.NewGuid().ToString(); 
            Console.WriteLine("Connect");
            client.Connect(clientId); 
 
            // subscribe to the topic "/halloween/skull/ping" with QoS 2 
            Console.WriteLine("Subscribe");
            client.Subscribe(new string[] { string.Format("{0}skull/ping",baseTopic) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            client.Subscribe(new string[] { string.Format("{0}skull/heady/{1}",baseTopic,deviceId) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            client.Subscribe(new string[] { string.Format("{0}skull/headx/{1}",baseTopic, deviceId) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            client.Subscribe(new string[] { string.Format("{0}skull/jaw/{1}",baseTopic, deviceId) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            client.Subscribe(new string[] { string.Format("{0}skull/laugh/{1}",baseTopic,deviceId) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            client.Subscribe(new string[] { string.Format("{0}skull/bow/{1}",baseTopic, deviceId) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
 
            Console.WriteLine("Done");
        }
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
        { 
            Console.WriteLine("Received: topic: {0} message: {1}", e.Topic,System.Text.Encoding.Default.GetString(e.Message));
            if (e.Topic==string.Format("{0}skull/ping",baseTopic))
            {
                client.Publish(string.Format("{0}pingr/{1}",baseTopic, deviceId), Encoding.UTF8.GetBytes(" "), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);   
            }
        } 
    }
}
