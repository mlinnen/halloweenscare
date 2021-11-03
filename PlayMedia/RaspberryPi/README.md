# Play Media on Raspberry Pi using Python
* The raspbian light OS can be used
* A basePath can be changed that points to a folder where all files become available for play.
* A path to the config.ini file can be changed.
* omxplayer is used to play all media. If you are using the raspbian lite distro you may need to install this on your Raspberry Pi.
* Use the HDMI port to play video on a projector or monitor.
* Use the audio jack to play the audio over some powered speakers.

## Media content
I bought the [AtmosFX Witching Hour Video](https://atmosfx.com/collections/atmosfearfx/products/witching-hour) for my setup.  The video content that this company creates is really fantastic.  You can get a DVD or download the content from their site after you make the purchase.  I have used a projector to play my video onto a a white bed sheet and I have also used the special screen that AtmosFX sells.  I prefer the screen from AtmosFX as it is less visible.  I have also used an LCD TV turned on it's side but viewing angles where a little narrow.

## Install MQTT for Python
The MQTT library is [Paho Client](https://eclipse.org/paho/clients/python/).  If you don't have pip3 installed on your raspberry pi you will need to install it using apt:
```
sudo apt install python3-pip
```

To install the MQTT library using pip:
```
pip3 install paho-mqtt
```

## Setup
1. Verify that omxplayer is on your raspberry pi, if it isn't then run the following to install it.
    ```
    sudo apt install omxplayer
    ```
1. Create a folder for the script, like /home/pi/halloween.
1. Copy the playvideo.py script and sample.ini file to the /home/pi/halloween folder.  
1. Create a folder to hold the media, like /home/pi/halloween/video.  
1. Copy the media to the new folder.

The python script is expecting the configuration values to be in **config.ini** so you need to rename the **sample.ini** to **config.ini** 
and then edit the **config.ini** file.  The **[mqtt_broker]** section in the INI file contains all the connection details to your broker.  
Currently you must use a broker that is configured with a username/password.  The **[media_player]** section of the INI file contains settings specific to the media player.  
* basePath - the location where the media content resides. 
* playerId - what uniquely identifies this player.  You can have more than 1 player on the network and have each of them play content independently of one another.
* root - the base of the topic that this play will subscribe and publish to.

At this point you might want to do some tests to make sure you can connect to the MQTT broker and receive commands.
1. Execute the playvideo.py script
    ```
    python3 playvideo.py /home/pi/halloween/config.ini
    ```
1. Using mosquitto sub utility subscribe to the root topic.  Of course make sure you change the broker connection details o match your environment.
    ```
    mosquitto_sub -h 192.168.1.5 -u username -P password -t /halloween/# -v
    ```
1. Using mosquitto pub utility send a ping command
    ```
    mosquitto_pub -h 192.168.1.5 -u username -P password -t /halloween/media/ping -m " "
    ```
1. You should get back a pingr from your device in mosquitto_sub

### Make the python script run on boot 
Systemd is a good way to get this script to run on boot.

1. Execute the following to create a configuration file for the python script.
    ```
    sudo nano /lib/systemd/system/playmedia.service
    ```
1. Add the following to the file.
    ```
    [Unit]
    Description=Play Media
    After=multi-user.target

    [Service]
    WorkingDirectory=/home/pi/
    User=pi
    Type=idle
    ExecStart=/usr/bin/python3 /home/pi/halloween/playvideo.py /home/pi/halloween/config.ini

    [Install]
    WantedBy=multi-user.target
    ```
1. Change the permission to the configuration file
    ```
    sudo chmod 644 /lib/systemd/system/playmedia.service
    ```
1. Tell systemd to start the process on startup
    ```
    sudo systemctl daemon-reload
    sudo systemctl enable playmedia.service
    ```
1. Reboot the Pi
    ```
    sudo reboot
    ```
1. Using mosquitto pub utility send a ping command to verify the pi is listening for MQTT messages.
    ```
    mosquitto_pub -h 192.168.1.5 -u username -P password -t /halloween/media/ping -m " "
    ```
1. You should get back a pingr from your device in mosquitto_sub

### Hide the raspberry pi boot messages
It is nice to not display the boot messages on your screen as the system comes up.  This requires changing some boot options.
```
sudo nano /boot/cmdline.txt
```
Then make the following changes in the file:
1. replace console=tty1 to console=tty3
1. add loglevel=3
1. add vt.global_cursor_default=0
1. execute the following to disable the console login on boot up
```
sudo systemctl disable getty@tty1.service
```

## MQTT Unsupported Topics and Messages
Everything is supported.

