using System;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using CommandLine;
using System.Collections.Generic;

namespace mqtthead
{
    class Program
    {
        static MqttClient _client;

        static Options _options;

        static void Main(string[] args)
        {

            // Allow for the following to be passed into the command line
            // Broker Ip address
            // Broker User Name and password
            // The base topic that should be used for subscribing and publishing.
            // The device Id
            // 
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>RunOptionsAndReturnExitCode(o))
                .WithNotParsed<Options>((errs) => HandleParseError(errs));

        }

        static void HandleParseError(IEnumerable<Error> errs)
		{
			
		}

        static int RunOptionsAndReturnExitCode(Options options)
		{
            _client = new MqttClient(options.BrokerIp);
            _options = options;

            // register to message received 
            _client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
 
            string clientId = Guid.NewGuid().ToString(); 
            Console.WriteLine("Connect");
            _client.Connect(clientId); 
 
            // subscribe to the topic "/halloween/skull/ping" with QoS 2 
            Console.WriteLine("Subscribe");
            _client.Subscribe(new string[] { string.Format("{0}skull/ping",options.BaseTopic) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            _client.Subscribe(new string[] { string.Format("{0}skull/heady/{1}",options.BaseTopic,options.Id) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            _client.Subscribe(new string[] { string.Format("{0}skull/headx/{1}",options.BaseTopic, options.Id) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            _client.Subscribe(new string[] { string.Format("{0}skull/jaw/{1}",options.BaseTopic, options.Id) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            _client.Subscribe(new string[] { string.Format("{0}skull/laugh/{1}",options.BaseTopic,options.Id) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
            _client.Subscribe(new string[] { string.Format("{0}skull/bow/{1}",options.BaseTopic, options.Id) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
 
            Console.WriteLine("Done");
			return 0;
		}

        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
        { 
            Console.WriteLine("Received: topic: {0} message: {1}", e.Topic,System.Text.Encoding.Default.GetString(e.Message));
            if (e.Topic==string.Format("{0}skull/ping",_options.BaseTopic))
            {
                _client.Publish(string.Format("{0}pingr/{1}",_options.BaseTopic, _options.Id), Encoding.UTF8.GetBytes(" "), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);   
            }
            else if (e.Topic==string.Format("{0}skull/heady/{1}",_options.BaseTopic,_options.Id))
            {
            }
            else if (e.Topic==string.Format("{0}skull/headx/{1}",_options.BaseTopic,_options.Id))
            {
            }
            else if (e.Topic==string.Format("{0}skull/jaw/{1}",_options.BaseTopic,_options.Id))
            {
            }
            else if (e.Topic==string.Format("{0}skull/laugh/{1}",_options.BaseTopic,_options.Id))
            {
            }
            else if (e.Topic==string.Format("{0}skull/bow/{1}",_options.BaseTopic,_options.Id))
            {
            }
        } 
    }
}
