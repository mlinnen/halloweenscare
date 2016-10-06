# Motion Detector on ESP8266
I used an Adafruit Feather HUZZAH as my ESP8266 device for this project.  This should work on other ESP8266 devices but I have not tried it on any other device.  Assuming you have cloned this repository then before you can open up the Sketch and compile it you will need to copy some header files and edit them to reflect your environment.

## Parts List:

1. [Adafruit Feather HUZZAH ESP8266](https://learn.adafruit.com/adafruit-feather-huzzah-esp8266/overview)
2. [HC-SR501 PIR Motion Sensor](https://www.amazon.com/gp/product/B00FDPO9B8/ref=ask_ql_qh_dp_hza)

## Wiring

1. Connect the Red wire (Power) of the Motion Sensor to PIN USB of the Feather HUZZAH.  This will power the sensor from the 5v coming from the USB Port.
    * Note: Some sensors can work off the 3.3 volt of the Huzzah but when I tried to do this with my sensor and I got a lot of false triggers.
2. Connect the Black wire (Ground) of the Motion Sensor to PIN GND of the Feather HUZZAH.
3. Connect the Yellow wire (Signal) of the Motion Sensor to PIN 12 of the Feather HUZZAH.

## Code

1. Copy **mywifi.sample.h** to **mywifi.h**
2. Edit **mywifi.h** to set the SSID for your Wifi connection as well as the password.
3. Copy **mymqttbroker.sample.h** to **mymqttbroker.sample.h**
4. Edit **mymqttbroker.sample.h** to set the broker ip, port, user name and password
5. Compile the sketch and upload it to the device.

The onboard LED should come on when Motion is detected and it should go off when motion stops.