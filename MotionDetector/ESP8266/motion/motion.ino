#include "mywifi.h"
#include "mymqttbroker.h"
#include <ESP8266WiFi.h>
#include <PubSubClient.h>

// Update these with values suitable for your network.

const char* ssid = WLAN_SSID;
const char* password = WLAN_PASS;
const char* mqtt_server = MQTT_SERVER;

const char* MQTT_TOPIC_PING = "/halloween/motion/ping";
const char* MQTT_TOPIC_PINGR = "/halloween/motion/pingr/1";
const char* MQTT_TOPIC_MOTION = "/halloween/motion/value/1";

WiFiClient espClient;
PubSubClient client(espClient);
long lastMsg = 0;
char msg[50];
int value = 0;

void setup_wifi() {

  delay(10);
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  randomSeed(micros());

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
}

void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();
  
  // If we get a ping request then respond to it
  if (strstr(topic,MQTT_TOPIC_PING)){
    client.publish(MQTT_TOPIC_PINGR, " ");
  }
}

void reconnect() {
  // Loop until we're reconnected
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID
    String clientId = "ESP8266Client-";
    clientId += String(random(0xffff), HEX);
    // Attempt to connect
    if (client.connect(clientId.c_str(),MQTT_USERNAME,MQTT_PASSWORD)) {
      Serial.println("connected");
      // Once connected, publish an announcement...
      client.publish(MQTT_TOPIC_MOTION, "0");
      // ... and resubscribe
      client.subscribe(MQTT_TOPIC_PING);
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}

void setup() {
  pinMode(BUILTIN_LED, OUTPUT);     // Initialize the BUILTIN_LED pin as an output
  Serial.begin(115200);
  setup_wifi();
  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);

  pinMode(12, INPUT);
}

int motion = 0;
int last_motion = 0;

void loop() {

  if (!client.connected()) {
    reconnect();
  }
  client.loop();

  motion = digitalRead(12);
  if (motion == HIGH && last_motion != motion){
    digitalWrite(BUILTIN_LED, LOW);
    Serial.println("Motion");
    client.publish(MQTT_TOPIC_MOTION, "1");
  }
  if (motion == LOW && last_motion != motion){
    digitalWrite(BUILTIN_LED, HIGH);
    Serial.println("No Motion");
    client.publish(MQTT_TOPIC_MOTION, "0");
  }

  last_motion = motion;
}
