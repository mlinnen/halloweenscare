# Motion Detector
The idea behind this project is to have a stand alone device that can detect motion and announce it using MQTT messaging.  You can use any device you want 
to actually detect motion and communicate via MQTT as long as you adhere to the following specifications: 

* The device supports configuring a **root** topic.  This will allow all commands to the device to be prefixed.

## MQTT Topics and Messages

**{root}** is your root topic that you want to use to isolate the device on the message bus. 
**{id}** is the Motion Detector Id you want to use to uniquely identify multiple Motion Detectors on the bus.  This **{id}** can be a number or characters as long as it is a set of MQTT Topic valid characters.

### MQTT Subscriptions
The following subscription topics should be supported by the device.  

Ask what Motion Detectors are online.  
```
/{root}/motion/ping
```

### MQTT Publications
The following topics and messages are published by this device when it's state has changed.

A Motion Detector publishes this when motion detection has changed.  The message body contains a 0 for no motion and a 1 for motion.
```
/{root}/motion/value/{id} 0 
```

A motion detector's response to the ping request.
```
/{root}/motion/pingr/{id} 
```

## Implementations 
* [ESP8266](ESP8266/)