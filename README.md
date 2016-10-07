# Halloween Scare
This repository contains several smaller projects that make up some of my Halloween decorations.

## Play Media
This device is designed to play a video or sound based on an MQTT message that comes in.
* [Play Media](PlayMedia/)

## Motion Detector
This device is designed to detect motion and publish an MQTT message.
* [Motion Detector](MotionDetector/)

## Fog Machine
This device is designed to control a fog machine via MQTT messages.
* [Fog Machine](FogMachine/)

## Scene Controller 
A scene controller is nothing more than a way to wire up multiple devices into a scene that is trigger by some external means.  The intent is to make the devices completely isolated from one another so that they only know what they do best and let the scene control manage gluing them all together.  MQTT again is used to subscribe and publish to these devices.  
* [Details about Scene Controllers](SceneController/)
   

## MQTT Broker
Most of these projects communicate with one another using [MQTT](http://www.mqtt.org).  You will need to setup an MQTT broker in order to 
have these devices talk to one another.  I recommend using [Mosquitto](https://mosquitto.org/) as you can run the 
broker on a Windows or Linux machine.  You can even run it on a Raspberry Pi.  I wont go into details 
on how to setup the broker so check out the [Mosquitto](https://mosquitto.org/) website for more details on broker install. 
