# Motion Detector on ESP8266
I used an [Adafruit Feather HUZZAH](https://learn.adafruit.com/adafruit-feather-huzzah-esp8266/overview) as my ESP8266 device for this project.  This should work on other ESP8266 devices but I have not tried it on any other device.  Assuming you have cloned this repository then before you can open up the Sketch and compile it you will need to copy some header files and edit them to reflect your environment.

1. Copy **mywifi.sample.h** to **mywifi.h**
2. Edit **mywifi.h** to set the SSID for your Wifi connection as well as the password.
3. Copy **mymqttbroker.sample.h** to **mymqttbroker.sample.h**
4. Edit **mymqttbroker.sample.h** to set the broker ip, port, user name and password
5. Compile the sketch and upload it to the device.