# Fog Machine
The idea behind this project is to have a stand alone device that can control a fog machine using MQTT messaging.  You can use any device you want 
to actually control the fog machine as long as you adhere to the following specifications: 

* The device supports configuring a **root** topic.  This will allow all commands to the device to be prefixed.
* The device knows how long the Fog Machine needs to charge between each fog release.
* The device ignores commands to turn on the fogger if it has not fully charged yet.

## MQTT Topics and Messages

**{root}** is your root topic that you want to use to isolate the device on the message bus. 
**{id}** is the Fog Machine Id you want to use to uniquely identify multiple Fog Machines on the bus.  This **{id}** can be a number or characters as long as it is a set of MQTT Topic valid characters.

### MQTT Subscriptions
The following subscription topics should be supported by the device.  

Ask what Fog Machines are online.  
```
/{root}/fog/ping
```

Turn the Fog Machine fog on or off.  The message body contains a 1 to turn it on and a 0 to turn it off.  
```
/{root}/fog/switch/{id} 1
```

Turn the Fog Machine fog on for X number of seconds.  The message body contains the number of seconds you want the fog to be turned on.    
```
/{root}/fog/pulse/{id} 30
```

Set the charge time that the Fog Machine needs between discharges.  The message body contains a number in seconds.  
```
/{root}/fog/chargetime/{id} 30
```

### MQTT Publications
The following topics and messages are published by this device when it's state has changed.

A Fog Machine publishes this when the state of the fog has changed.  The message body contains a 0 for no fog and a 1 for fog.
```
/{root}/fog/on/{id} 0 
```

A fog machine's response to the ping request.
```
/{root}/fog/pingr/{id} 
```

## Implementations 
* [ESP8266](ESP8266/)
