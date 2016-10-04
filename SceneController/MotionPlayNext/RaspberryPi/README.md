# Scene Controller 
## Motion Play Next on Raspberry Pi using Python

## Install MQTT for Python
The MQTT library is [Paho Client](https://eclipse.org/paho/clients/python/).  
To install the library using pip:
```
pip3 install paho-mqtt
```

## Motion Play Next (motionplaynext.py)
This is a simple scene that watches for the motion MQTT message and when motion is detected it will send a playnext command to the media player.  

### Setup
The python script is expecting the configuration values to be in **config.ini** so you need to copy **sample.ini** to **config.ini** 
and then edit the **config.ini** file.  The **[mqtt_broker]** section in the INI file contains all the connection details to your broker.  
Currently you must use a broker that is configured with a username/password.  The **[scene]** section of the INI file contains settings 
specific to the scene.  
* root - the base of the topic that this play will subscribe and publish to.
* motionId - the motionid we want to monitor to detect that motion has occurred.
* playerId - the id of the player that we want to send the playnext command to.

