using CommandLine;
public class Options
{
    [Option('b', "brokerIp", Required = true, HelpText = "Set the MQTT broker IP")]
    public string BrokerIp { get; set; }

    [Option('u', "userName", Required = true, HelpText = "Set the MQTT broker User Name")]
    public string UserName { get; set; }

    [Option('p', "password", Required = true, HelpText = "Set the MQTT broker password")]
    public string Password { get; set; }

    [Option('i', "id", Required = true, HelpText = "Set the device id")]
    public string Id { get; set; }

    [Option('t', "topic", Required = true, HelpText = "Set the base topic")]
    public string BaseTopic { get; set; }
}