# Halloween Scare
This repository contains several smaller projects that make up some of my Halloween decorations.

## Play Media
This project is designed to play a video or sound based on an MQTT message that comes in.

## Motion Detector
This project is designed to detect motion and publish an MQTT message.

## Scene Controller (Coming Soon)
This project is the glue between the devices.  You can have individual devices sending commands directly to other devices but I like to make my 
devices stand alone and have a controller orchestrait the whole scene by interperating sensor data and determining what devices should be triggerd.  
The controller basically listens for MQTT messages and decides what MQTT messages need to be published to fire the animatonic devices.   

## MQTT Broker
Most of these projects communicate with one another using [MQTT](http://www.mqtt.org).  You will need to setup an MQTT broker in order to 
have these devices talk to one another.  I recommend using [Mosquitto](https://mosquitto.org/) as you can run the 
broker on a Windows or Linux machine.  You can even run it on a Raspberry Pi.  I wont go into details 
on how to setup the broker so check out the [Mosquitto](https://mosquitto.org/) website for more details on broker install. 
