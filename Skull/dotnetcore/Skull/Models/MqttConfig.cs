namespace Skull
{
    public class MqttConfig
    {
        public MqttConfig()
        {
        }

        /// <summary>
        /// The broker ip
        /// </summary>
        public string Broker { get; set; }

        /// <summary>
        /// The user name for logging into the broker
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The password for logging into the broker
        /// </summary>
        public string  Password { get; set; }

        /// <summary>
        /// The basic topic prefix for all publish and subscribe operations on the mqtt bus
        /// </summary>
        public string BaseTopic { get; set; }

        /// <summary>
        /// The Skull Id (allows for multiple skulls to exist on the bus and you can controll all of them at once)
        /// </summary>
        public int TopicId { get; set; }
    }

}