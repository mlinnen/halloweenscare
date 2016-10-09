# Fog Machine on ESP8266
I used an Adafruit Feather HUZZAH as my ESP8266 device for this project.  This should work on other ESP8266 devices but I have not tried it on any other device.  Assuming you have cloned this repository then before you can open up the Sketch and compile it you will need to copy some header files and edit them to reflect your environment.

## Parts List:

1. [Adafruit HUZZAH ESP8266](https://www.adafruit.com/products/2471)
2. [SainSmart 2-Channel 5v Relay Module](http://www.sainsmart.com/arduino-pro-mini.html)
3. Fog Machine

### Fog Machine
For this to work you will need to have a Fog Machine that has a remote switch that dispenses the fog.  You will need to open up the remote and connect the relay contacts across the switch. The relay is just going to take the place of the switch but you can still leave the switch in place so that you can manually trigger the fog if you want to.  WARNING be very careful using this device as the relay module and the Fog Machine remote might have 120 volt AC on it so never handle either of these devices while the Fog Machine is plugged in.  

## Wiring

1. Connect the (VCC) of the Relay Module to PIN USB of the Feather HUZZAH.  This will power the Relay Module from the 5v coming from the USB Port.
2. Connect the (GND) of the Relay Module to PIN GND of the Feather HUZZAH.
3. Connect the (IN1) of the Relay Module to PIN 12 of the Feather HUZZAH.

## Code

1. Copy **mywifi.sample.h** to **mywifi.h**
2. Edit **mywifi.h** to set the SSID for your Wifi connection as well as the password.
3. Copy **mymqttbroker.sample.h** to **mymqttbroker.sample.h**
4. Edit **mymqttbroker.sample.h** to set the broker ip, port, user name and password
5. Compile the sketch and upload it to the device.

The onboard LED should come on when Fog Machine is turned on and it should go off when the Fog Machine is turned off.

## MQTT Unsupported Messages and Topics
Currently this implementation of the Fog Machine does not support the following:

Charge Time  
```
/{root}/fog/chargetime/{id} 30
```
