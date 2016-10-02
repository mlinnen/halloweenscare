import sys
import logging
import configparser
import paho.mqtt.client as mqtt

# Read INI file for all the configuration
parser = configparser.ConfigParser()
parser.read('config.ini')
mqtt_broker_ip = parser.get('mqtt_broker', 'ip')
mqtt_broker_port = parser.getint('mqtt_broker', 'port')
mqtt_broker_username = parser.get('mqtt_broker', 'username')
mqtt_broker_password = parser.get('mqtt_broker', 'password')

mqtt_broker_root = parser.get('scene', 'root')
playerId = parser.get('scene', 'playerid')
motionId = parser.get('scene', 'motionid')

logging.basicConfig(stream=sys.stderr, level=logging.DEBUG)

# The callback for when the client receives a CONNACK response from the server.
def on_connect(client, userdata, rc):
    print("Connected with result code "+str(rc))
    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    client.subscribe(mqtt_broker_root + "motion/value/" + playerId )

# The callback for when a PUBLISH message is received from the server.
def on_message(client, userdata, msg):
    if ("/motion/value/" in msg.topic):
        print(msg.payload)
        if (msg.payload==b'1'):
            print("Motion detected")
            client.publish(mqtt_broker_root + "media/playnext/"  + playerId, "", qos=1)

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

