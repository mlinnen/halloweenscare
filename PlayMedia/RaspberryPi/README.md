# Play Media on Raspberry Pi using Python
* A basePath can be changed that points to a folder where all files become available for play.
* A path to the config.ini file can be changed.
* omxplayer is used to play all media.
* Use the HDMI port to play video on a projector or monitor.
* Use the audio jack to play the audio over some powered speakers.

## Media content
I bought the [AtmosFX Witching Hour Video](https://atmosfx.com/collections/atmosfearfx/products/witching-hour) for my setup.  The video content that this company creates is really fantastic.  You can get a DVD or download the content from their site after you make the purchase.  I chose to project my video onto a window using a sheet.    

## Install MQTT for Python
The MQTT library is [Paho Client](https://eclipse.org/paho/clients/python/).  
To install the library using pip:
```
pip3 install paho-mqtt
```

## Setup
The python script is expecting the configuration values to be in **/home/pi/halloween/config.ini** so you need to copy the **sample.ini** to **/home/pi/halloween/config.ini** 
and then edit the **config.ini** file.  The **[mqtt_broker]** section in the INI file contains all the connection details to your broker.  
Currently you must use a broker that is configured with a username/password.  The **[media_player]** section of the INI file contains settings 
specific to the media player.  
* basePath - the location where the media content resides. 
* playerId - what uniquely identifies this player.  You can have more than 1 player on the network and have each of them play content independently of one another.
* root - the base of the topic that this play will subscribe and publish to.

## MQTT Unsupported Topics and Messages
The following topic is currently not supported yet.  

```
/{root}/media/playended/{id} 1
```

## Execution
To run the script and start listening for MQTT messages open up a shell prompt and navigate to the directory where you cloned the repository. Under PlayMedia/RaspberryPi run the following: 
```
python3 playvideo.py
```
