import RPi.GPIO as GPIO
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

# Read INI file for all the configuration
parser = configparser.ConfigParser()
parser.read('config.ini')
mqtt_broker_ip = parser.get('mqtt_broker', 'ip')
mqtt_broker_port = parser.getint('mqtt_broker', 'port')
mqtt_broker_username = parser.get('mqtt_broker', 'username')
mqtt_broker_password = parser.get('mqtt_broker', 'password')

mqtt_broker_root = parser.get('media_player', 'root')
basePath = parser.get('media_player', 'basePath')
playerId = parser.get('media_player', 'playerId')

logging.basicConfig(stream=sys.stderr, level=logging.DEBUG)

# The callback for when the client receives a CONNACK response from the server.
def on_connect(client, userdata, rc):
    print("Connected with result code "+str(rc))
    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    client.subscribe(mqtt_broker_root + "media/ping")
    client.subscribe(mqtt_broker_root + "media/items/" + playerId)
    client.subscribe(mqtt_broker_root + "media/play/" + playerId)
    client.subscribe(mqtt_broker_root + "media/playnext/" + playerId)
    client.subscribe(mqtt_broker_root + "media/stopall/")
    client.subscribe(mqtt_broker_root + "media/stop/" + playerId)

# The callback for when a PUBLISH message is received from the server.
def on_message(client, userdata, msg):
    global omxc
    global player
    global index
    
    if ("/play/" in msg.topic):
        print("play")
        # Check to see if the player is still running
        if omxc is not None:
            if omxc.poll() != None:
                omxc = None
                player = False
        
        if (not player):
            i = int(msg.payload)
            if (i>-1 and i<len(movies)):
                index = i
                logging.debug(movies[index])
                omxc = subprocess.Popen(['omxplayer', '-b','-o','local', movies[index]], stdin=subprocess.PIPE, stdout=subprocess.PIPE)
                client.publish(mqtt_broker_root + "media/playstarted/"  + playerId, index, qos=1)
                time.sleep(2)
                player = True
                index += 1
                if (len(movies) <= index):
                    index=0        
                
        
    if ("/items/" in msg.topic):
        client.publish(mqtt_broker_root + "media/itemsr/"  + playerId, len(movies), qos=1)
        print("items")
    if ("/playnext/" in msg.topic):
        print("playnext")
        # Check to see if the player is still running
        if omxc is not None:
            if omxc.poll() != None:
                omxc = None
                player = False

        if (not player):
            logging.debug(movies[index])
            omxc = subprocess.Popen(['omxplayer', '-b','-o','local', movies[index]], stdin=subprocess.PIPE, stdout=subprocess.PIPE)
            time.sleep(2)
            player = True
            index += 1
            if (len(movies) <= index):
                index=0        
        
    if ("/stop/" in msg.topic or "/stopall" in msg.topic):
        print("stop")
        if (player == True):
            if omxc is not None:
                omxc.stdin.write(b'q')
                omxc.stdin.flush()
                time.sleep(1)
    if ("/media/ping" in msg.topic):
        print("ping")
        client.publish(mqtt_broker_root + "media/pingr/"  + playerId, "", qos=1)

# Find all files in the basePath
movies = []
for (dirpath, dirnames, filenames) in os.walk(basePath):
    for filename in filenames:
        movies.append(os.path.join(basePath,filename))
    break


client = mqtt.Client()
client.username_pw_set(mqtt_broker_username,mqtt_broker_password )
client.on_connect = on_connect
client.on_message = on_message

client.connect(mqtt_broker_ip, mqtt_broker_port, 60)

# Blocking call that processes network traffic, dispatches callbacks and
# handles reconnecting.
# Other loop*() functions are available that give a threaded interface and a
# manual interface.
client.loop_forever()

