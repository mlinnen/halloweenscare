#! /usr/bin/env python3
import os
import sys
import subprocess
import logging
import time
import configparser
import paho.mqtt.client as mqtt

player = False
omxc = None
index = 0
playingIndex = 0

# Read INI file for all the configuration
parser = configparser.ConfigParser()
# Change where you want the location of the config to be
parser.read('/home/pi/halloween/config.ini')
mqtt_broker_ip = parser.get('mqtt_broker', 'ip')
mqtt_broker_port = parser.getint('mqtt_broker', 'port')
mqtt_broker_username = parser.get('mqtt_broker', 'username')
mqtt_broker_password = parser.get('mqtt_broker', 'password')

mqtt_broker_root = parser.get('media_player', 'root')
basePath = parser.get('media_player', 'basePath')
playerId = parser.get('media_player', 'playerId')
loggingLevel = parser.get('logging', 'level')

if ("warning" in loggingLevel):
    logging.basicConfig(stream=sys.stderr, level=logging.WARNING)
elif ("info" in loggingLevel):
    logging.basicConfig(stream=sys.stderr, level=logging.INFO)
else:
    logging.basicConfig(stream=sys.stderr, level=logging.DEBUG)

# The callback for when the client receives a CONNACK response from the server.
def on_connect(self, client, userdata, rc):
    logging.debug("Connected with result code "+str(rc))

    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    self.subscribe(mqtt_broker_root + "media/ping")
    self.subscribe(mqtt_broker_root + "media/items/" + playerId)
    self.subscribe(mqtt_broker_root + "media/play/" + playerId)
    self.subscribe(mqtt_broker_root + "media/playnext/" + playerId)
    self.subscribe(mqtt_broker_root + "media/stopall")
    self.subscribe(mqtt_broker_root + "media/stop/" + playerId)

# The callback for when a PUBLISH message is received from the server.
def on_message(client, userdata, message):
    global omxc
    global player
    global index

    if ("/play/" in message.topic):
        logging.debug("play")
        # Check to see if the player is still running
        if omxc is not None:
            if omxc.poll() != None:
                omxc = None
                player = False

        if (not player):
            i = int(message.payload) - 1
            if (i>-1 and i<len(movies)):
                index = i
                logging.debug(movies[index])
                omxc = subprocess.Popen(['omxplayer', '-b','-o','local', movies[index]], stdin=subprocess.PIPE, stdout=subprocess.PIPE)
                time.sleep(2)
                playingIndex = index+1
                client.publish(mqtt_broker_root + "media/playstarted/"  + playerId, playingIndex, qos=1)
                player = True
                index += 1
                if (len(movies) <= index):
                    index=0


    if ("/items/" in message.topic):
        client.publish(mqtt_broker_root + "media/itemsr/"  + playerId, str(len(movies)), qos=1)
        logging.debug("items " + str(len(movies)))
    if ("/playnext/" in message.topic):
        # Check to see if the player is still running
        if omxc is not None:
            if omxc.poll() != None:
                omxc = None
                player = False

        if (not player):
            logging.debug(movies[index])
            omxc = subprocess.Popen(['omxplayer', '-b','-o','local', movies[index]], stdin=subprocess.PIPE, stdout=subprocess.PIPE)
            time.sleep(2)
            playingIndex = index+1
            client.publish(mqtt_broker_root + "media/playstarted/"  + playerId, playingIndex, qos=1)
            player = True
            index += 1
            if (len(movies) <= index):
                index=0

    if ("/stop/" in message.topic or "/stopall" in message.topic):
        logging.debug("stop")
        if (player == True):
            if omxc is not None:
                omxc.stdin.write(b'q')
                omxc.stdin.flush()
                time.sleep(1)
    if ("/media/ping" in message.topic):
        logging.debug("ping")
        client.publish(mqtt_broker_root + "media/pingr/"  + playerId, "", qos=1)
        print("\033c") # Clear the console

# Find all files in the basePath
movies = []
for (dirpath, dirnames, filenames) in os.walk(basePath):
    for filename in sorted(filenames):
        movies.append(os.path.join(basePath,filename))
    break


client = mqtt.Client()
client.username_pw_set(mqtt_broker_username,mqtt_broker_password )
client.on_connect = on_connect
client.on_message = on_message

client.connect(mqtt_broker_ip, mqtt_broker_port, 60)

run = True
while run:
    client.loop()

    # If a video was started then look for it being completed
    if omxc is not None:
        if omxc.poll() != None:
            print("\033c") # Clear the console
            omxc = None
            player = False
            client.publish(mqtt_broker_root + "media/playended/"  + playerId, playingIndex, qos=1)


