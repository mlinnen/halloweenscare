#! /usr/bin/env python3
import os
import sys
import subprocess
import logging
import time
import argparse
import configparser
import paho.mqtt.client as mqtt

player = False
omxc = None
index = 0
playingIndex = 0
retry=0
retry_limit=9
retry_delay_fixed=2
connected_once=False
count=0
stime=time.time()
retry_delay=retry_delay_fixed

argParser = argparse.ArgumentParser(description='Play Media')
argParser.add_argument('file', help='Path to the config filename')
args = argParser.parse_args()

# Read INI file for all the configuration
parser = configparser.ConfigParser()
# Change where you want the location of the config to be
# parser.read('/home/pi/halloween/config.ini')
if args.file:
    parser.read(args.file)
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
def on_connect(client, userdata, flags, rc):
    logging.debug("on_connect rc:"+str(rc))
    if rc==0:
        logging.info("Connected to broker "+mqtt_broker_ip+":"+str(mqtt_broker_port))
        client.connected_flag=True

        # Subscribing in on_connect() means that if we lose the connection and
        # reconnect then subscriptions will be renewed.
        client.subscribe(mqtt_broker_root + "media/ping")
        client.subscribe(mqtt_broker_root + "media/items/" + playerId)
        client.subscribe(mqtt_broker_root + "media/play/" + playerId)
        client.subscribe(mqtt_broker_root + "media/playnext/" + playerId)
        client.subscribe(mqtt_broker_root + "media/stopall")
        client.subscribe(mqtt_broker_root + "media/stop/" + playerId)
    else:
        client.connected_flag=False
        client.bad_connection_flag=True
        logging.error("Unable to connect to broker "+mqtt_broker_ip+":"+str(mqtt_broker_port)+" result code "+str(rc))
        sys.exit(1) #quit

def on_disconnect(client, userdata, rc):
    logging.info("Disconnecting with result code  "+str(rc))
    client.connected_flag=False

# The callback for when a PUBLISH message is received from the server.
def on_message(client, userdata, message):
    global omxc
    global player
    global index
    global playingIndex

    if ("/play/" in message.topic):
        logging.debug("play")
        # Check to see if the player is still running
        if omxc is not None:
            if omxc.poll() != None:
                omxc = None
                player = False

        if (not player):
            i = int(message.payload) - 1
            if (i>-1 and i<len(mediaItems)):
                index = i
                logging.debug(mediaItems[index])
                omxc = subprocess.Popen(['omxplayer', '-b','-o','local', mediaItems[index]], stdin=subprocess.PIPE, stdout=subprocess.PIPE)
                time.sleep(2)
                playingIndex = index + 1
                client.publish(mqtt_broker_root + "media/playstarted/"  + playerId, playingIndex, qos=1)
                player = True
                index += 1
                if (len(mediaItems) <= index):
                    index=0


    if ("/items/" in message.topic):
        client.publish(mqtt_broker_root + "media/itemsr/"  + playerId, str(len(mediaItems)), qos=1)
        logging.debug("items " + str(len(mediaItems)))
    if ("/playnext/" in message.topic):
        # Check to see if the player is still running
        if omxc is not None:
            if omxc.poll() != None:
                omxc = None
                player = False

        if (not player):
            logging.debug(mediaItems[index])
            omxc = subprocess.Popen(['omxplayer', '-b','-o','local', mediaItems[index]], stdin=subprocess.PIPE, stdout=subprocess.PIPE)
            time.sleep(2)
            playingIndex = index + 1
            client.publish(mqtt_broker_root + "media/playstarted/"  + playerId, playingIndex, qos=1)
            player = True
            index += 1
            if (len(mediaItems) <= index):
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
mediaItems = []
for (dirpath, dirnames, filenames) in os.walk(basePath):
    for filename in sorted(filenames):
        mediaItems.append(os.path.join(basePath,filename))
    break


client = mqtt.Client()
mqtt.Client.connected_flag=False
mqtt.Client.bad_connection_flag=False #
mqtt.Client.retry_count=0 #
client.username_pw_set(mqtt_broker_username,mqtt_broker_password )
client.on_connect = on_connect
client.on_disconnect = on_disconnect
client.on_message = on_message

run = True
counter = 0
logging.debug("Starting loop")
while run:
    client.loop(0.01)

    if client.connected_flag:
        # If a video was started then look for it being completed
        if omxc is not None:
            if omxc.poll() != None:
                print("\033c") # Clear the console
                omxc = None
                player = False
                client.publish(mqtt_broker_root + "media/playended/"  + playerId, playingIndex, qos=1)

    rdelay=time.time()-stime

    if not client.connected_flag and rdelay>retry_delay:
        logging.info("rdelay=" + str(rdelay))
        try:
            retry+=1
            if connected_once:
                logging.info("Reconnecting attempt Number="+str(retry))
            else:
                logging.info("Connecting attempt Number="+str(retry))

            client.connect(mqtt_broker_ip, mqtt_broker_port)
            
            while not client.connected_flag:
                client.loop(0.01)
                time.sleep(1) #wait for connection to complete
                stime=time.time()
                retry_delay=retry_delay_fixed
            connected_once=True
            retry=0 #reset
        except Exception as e:
            logging.error("\nConnect failed : "+str(e))
            retry_delay=retry_delay*retry_delay
            if retry_delay>100:
                retry_delay=100
            logging.info("retry Interval =" + str(retry_delay))
            if retry>retry_limit:
                sys.exit(1)
