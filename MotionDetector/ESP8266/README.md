# Motion Detector on ESP8266
I used an Adafruit Feather HUZZAH as my ESP8266 device for this project.  This should work on other ESP8266 devices but I have not tried it on any other device.  Assuming you have cloned this repository then before you can open up the Sketch and compile it you will need to copy some header files and edit them to reflect your environment.

## Parts List:

1. [Adafruit Feather HUZZAH ESP8266](https://learn.adafruit.com/adafruit-feather-huzzah-esp8266/overview)
2. [PIR Motion Sensor]( https://www.adafruit.com/products/189)

## Wiring

1. Connect the Red wire of the Motion Sensor to PIN 3V of the Feather HUZZAH.  This will power the sensor from the 3.3v regulator.
2. Connect the Black wire of the Motion Sensor to PIN GND of the Feather HUZZAH.
3. Connect the Yellow wire of the Motion Sensor to PIN 12 of the Feather HUZZAH.

## Code

1. Copy **mywifi.sample.h** to **mywifi.h**
2. Edit **mywifi.h** to set the SSID for your Wifi connection as well as the password.
3. Copy **mymqttbroker.sample.h** to **mymqttbroker.sample.h**
4. Edit **mymqttbroker.sample.h** to set the broker ip, port, user name and password
5. Compile the sketch and upload it to the device.

The onboard LED should come on when Motion is detected and it should go off when motion stops.