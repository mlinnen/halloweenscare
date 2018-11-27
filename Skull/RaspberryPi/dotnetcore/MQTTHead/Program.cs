using System;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using CommandLine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace mqtthead
{
    class Program
    {
        static MqttClient _client;
        static string _clientId = Guid.NewGuid().ToString();
        static ILogger _logger;
        static Options _options;

        static void Main(string[] args)
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
              .AddConsole();
            _logger = loggerFactory.CreateLogger<Program>();

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o => RunOptionsAndReturnExitCode(o))
                .WithNotParsed<Options>((errs) => HandleParseError(errs));
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            _logger.Log(LogLevel.Error, "Error parsing the command line options");
            foreach (var error in errs)
            {
                Console.WriteLine(error);
            }
        }

        static int RunOptionsAndReturnExitCode(Options options)
        {
            _client = new MqttClient(options.BrokerIp);
            _options = options;
            if (!options.BaseTopic.StartsWith("/"))
                options.BaseTopic = string.Format("/{0}", options.BaseTopic);
            if (!options.BaseTopic.EndsWith("/"))
                options.BaseTopic = string.Format("{0}/", options.BaseTopic);

            // register for message received 
            _client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            _client.ConnectionClosed += client_ConnectionClosed;

            _logger.Log(LogLevel.Information, "Connecting to...{0}", _options.BrokerIp);
            _client.Connect(_clientId);

            // subscribe to the topic "/halloween/skull/ping" with QoS 2 
            _logger.Log(LogLevel.Information, "Subscribing");
            _client.Subscribe(new string[] { string.Format("{0}skull/ping", options.BaseTopic) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            _client.Subscribe(new string[] { string.Format("{0}skull/heady/{1}", options.BaseTopic, options.Id) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            _client.Subscribe(new string[] { string.Format("{0}skull/headx/{1}", options.BaseTopic, options.Id) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            _client.Subscribe(new string[] { string.Format("{0}skull/jaw/{1}", options.BaseTopic, options.Id) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            _client.Subscribe(new string[] { string.Format("{0}skull/laugh/{1}", options.BaseTopic, options.Id) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            _client.Subscribe(new string[] { string.Format("{0}skull/bow/{1}", options.BaseTopic, options.Id) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            _logger.Log(LogLevel.Information, "Connected to...{0}", _options.BrokerIp);

            Console.CancelKeyPress += delegate {
                _logger.Log(LogLevel.Information, "Closing the application");
                // Unsubscribe from MQTT topics
                _client.Unsubscribe(new string[] { string.Format("{0}skull/ping", options.BaseTopic),
                                                   string.Format("{0}skull/heady/{1}", options.BaseTopic, options.Id),
                                                   string.Format("{0}skull/headx/{1}", options.BaseTopic, options.Id),
                                                   string.Format("{0}skull/jaw/{1}", options.BaseTopic, options.Id),
                                                   string.Format("{0}skull/laugh/{1}", options.BaseTopic, options.Id),
                                                   string.Format("{0}skull/bow/{1}", options.BaseTopic, options.Id) });
                // Disconnect from the MQTT broker
                _client.Disconnect();
            };

            return 0;
        }
        static async Task TryReconnectAsync(CancellationToken cancellationToken)
        {
            var connected = _client.IsConnected;
            while (!connected && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _client.Connect(_clientId);
                }
                catch
                {
                    _logger.Log(LogLevel.Warning, "No connection to...{0}", _options.BrokerIp);
                }
                connected = _client.IsConnected;
                await Task.Delay(10000, cancellationToken);
            }
        }
        static void client_ConnectionClosed(object sender, EventArgs e)
        {
            _logger.Log(LogLevel.Warning, "Connection lost");
        }

        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            _logger.Log(LogLevel.Information, "Received: topic: {0} message: {1}",  e.Topic, System.Text.Encoding.Default.GetString(e.Message));
            if (e.Topic == string.Format("{0}skull/ping", _options.BaseTopic))
            {
                // Reply to the ping request
                _client.Publish(string.Format("{0}pingr/{1}", _options.BaseTopic, _options.Id), Encoding.UTF8.GetBytes(" "), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            else if (e.Topic == string.Format("{0}skull/heady/{1}", _options.BaseTopic, _options.Id))
            {
                // TODO move the y axis
            }
            else if (e.Topic == string.Format("{0}skull/headx/{1}", _options.BaseTopic, _options.Id))
            {
                // TODO move the x axis
            }
            else if (e.Topic == string.Format("{0}skull/jaw/{1}", _options.BaseTopic, _options.Id))
            {
                // TODO move the jaw
            }
            else if (e.Topic == string.Format("{0}skull/laugh/{1}", _options.BaseTopic, _options.Id))
            {
                // TODO execute the laugh
            }
            else if (e.Topic == string.Format("{0}skull/bow/{1}", _options.BaseTopic, _options.Id))
            {
                // TODO bow the head
            }
        }
    }
}
