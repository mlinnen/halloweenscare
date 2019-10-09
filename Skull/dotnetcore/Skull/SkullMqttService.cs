
using System;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Skull
{
    public class SkullMqttService
    {
        private readonly ILogger _logger;
        private readonly MqttConfig _mqttConfig;
        private readonly CommandQueue _commandQueue;
        private MqttClient _client;
        private string _clientId;


        public SkullMqttService(ILogger<SkullMqttService> logger, MqttConfig mqttConfig, CommandQueue commandQueue)
        {
            _logger = logger;
            _mqttConfig = mqttConfig;
            _commandQueue = commandQueue;
        }

        public void Connect()
        {
            _logger.Log(LogLevel.Information, "Attempting to Connecte to the Mqtt Broker: {0}",_mqttConfig.Broker);

            _clientId = Guid.NewGuid().ToString();

             _client = new MqttClient(_mqttConfig.Broker);
            _client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            _client.ConnectionClosed += client_ConnectionClosed;

            _client.Connect(_clientId);
            _client.Subscribe(new string[] { string.Format("{0}skull/ping", _mqttConfig.BaseTopic) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            _client.Subscribe(new string[] { string.Format("{0}skull/heady/{1}", _mqttConfig.BaseTopic, _mqttConfig.TopicId) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            _client.Subscribe(new string[] { string.Format("{0}skull/headx/{1}", _mqttConfig.BaseTopic, _mqttConfig.TopicId) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            _client.Subscribe(new string[] { string.Format("{0}skull/jaw/{1}", _mqttConfig.BaseTopic, _mqttConfig.TopicId) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            _client.Subscribe(new string[] { string.Format("{0}skull/laugh/{1}", _mqttConfig.BaseTopic, _mqttConfig.TopicId) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            _client.Subscribe(new string[] { string.Format("{0}skull/bow/{1}", _mqttConfig.BaseTopic, _mqttConfig.TopicId) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            _logger.Log(LogLevel.Information, "Connected to...{0}", _mqttConfig.Broker);

        }

        public void Disconnect()
        {
            _client.Unsubscribe(new string[] { string.Format("{0}skull/ping", _mqttConfig.BaseTopic),
                                                   string.Format("{0}skull/heady/{1}", _mqttConfig.BaseTopic, _clientId),
                                                   string.Format("{0}skull/headx/{1}", _mqttConfig.BaseTopic, _clientId),
                                                   string.Format("{0}skull/jaw/{1}", _mqttConfig.BaseTopic, _clientId),
                                                   string.Format("{0}skull/laugh/{1}", _mqttConfig.BaseTopic, _clientId),
                                                   string.Format("{0}skull/bow/{1}", _mqttConfig.BaseTopic, _clientId) });
            _client.Disconnect();
        }

        private void client_ConnectionClosed(object sender, EventArgs e)
        {
            _logger.Log(LogLevel.Warning, "Connection lost");
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string payload = System.Text.Encoding.Default.GetString(e.Message);
            _logger.Log(LogLevel.Information, "Received: topic: {0} message: {1}",  e.Topic, payload);
            if (e.Topic == string.Format("{0}skull/ping", _mqttConfig.BaseTopic))
            {
                // Reply to the ping request
                _client.Publish(string.Format("{0}pingr/{1}", _mqttConfig.BaseTopic, _mqttConfig.TopicId), Encoding.UTF8.GetBytes(" "), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            else if (e.Topic == string.Format("{0}skull/heady/{1}", _mqttConfig.BaseTopic, _mqttConfig.TopicId))
            {
                int value = 0;
                if (int.TryParse(payload, out value))
                {
                    var cmd = new CommandHeadY();
                    cmd.Value = value;
                    _commandQueue.Enqueue(cmd);
                }
            }
            else if (e.Topic == string.Format("{0}skull/headx/{1}", _mqttConfig.BaseTopic, _mqttConfig.TopicId))
            {
                int value = 0;
                if (int.TryParse(payload, out value))
                {
                    var cmd = new CommandHeadX();
                    cmd.Value = value;
                    _commandQueue.Enqueue(cmd);
                }
            }
            else if (e.Topic == string.Format("{0}skull/jaw/{1}", _mqttConfig.BaseTopic, _mqttConfig.TopicId))
            {
                int value = 0;
                if (int.TryParse(payload, out value))
                {
                    var cmd = new CommandJaw();
                    cmd.Value = value;
                    _commandQueue.Enqueue(cmd);
                }
            }
            else if (e.Topic == string.Format("{0}skull/laugh/{1}", _mqttConfig.BaseTopic, _mqttConfig.TopicId))
            {
                int delay = 0;
                int numberOfTimes = 0;
                string[] parameters = payload.Split(' ');
                if (parameters.Length>=1)
                    int.TryParse(parameters[0].Trim(), out delay);
                if (parameters.Length >= 2)
                    int.TryParse(parameters[1].Trim(), out numberOfTimes);

                var cmd = new CommandLaugh();
                cmd.StartDelay = delay;
                cmd.NumberOfTimes = numberOfTimes;
                _commandQueue.Enqueue(cmd);
            }
            else if (e.Topic == string.Format("{0}skull/bow/{1}", _mqttConfig.BaseTopic, _mqttConfig.TopicId))
            {
                var cmd = new CommandBow();
                _commandQueue.Enqueue(cmd);
            }
        }

    }
}