#include "mywifi.h"
#include "mymqttbroker.h"
#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#include "ctype.h"

const char* ssid = WLAN_SSID;
const char* password = WLAN_PASS;
const char* mqtt_server = MQTT_SERVER;

const char* MQTT_TOPIC_PING = "/halloween/fog/ping";
const char* MQTT_TOPIC_PINGR = "/halloween/fog/pingr/1";
const char* MQTT_TOPIC_ON = "/halloween/fog/on/1";
const char* MQTT_TOPIC_SWITCH = "/halloween/fog/switch/1";
const char* MQTT_TOPIC_PULSE = "/halloween/fog/pulse/1";
const char* MQTT_TOPIC_CHARGE = "/halloween/fog/charge/1";

const int RELAY_PIN = 12;

const int STATE_UNKNOWN = -1;
const int STATE_STARTUP = 0;
const int STATE_IDLE = 1;
const int STATE_FOGGING = 2;
const int STATE_CHARGING = 3;

WiFiClient espClient;
PubSubClient client(espClient);
long lastMsg = 0;
char msg[50];
int value = 0;
int newState = STATE_STARTUP;
int currentState = STATE_UNKNOWN;
boolean charging = false;
boolean chargingTimedOut = true;
boolean isPulseCommand = false;
int timerSec = 0;
int maxChargeTimeSec = 30;
int maxFogTimeSec = 30;
int last_fogon = -1;
boolean timedOut = true;
unsigned long timer;
unsigned long interval = 5000;
char message_buff[20];

void printState(int state) {
  if (state==STATE_IDLE){
     Serial.print("IDLE");
  }
  if (state==STATE_UNKNOWN){
     Serial.print("UNKNOWN");
  }
  if (state==STATE_STARTUP){
     Serial.print("STARTUP");
  }
  if (state==STATE_FOGGING){
     Serial.print("FOGGING");
  }
  if (state==STATE_CHARGING){
     Serial.print("CHARGING");
  }
}

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
  int i = 0;
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();

  // create character buffer with ending null terminator (string)
  for(i=0; i<length; i++) {
    message_buff[i] = payload[i];
  }
  message_buff[i] = '\0';

  String msgString = String(message_buff);
  
  // If we get a ping request then respond to it
  if (strstr(topic,MQTT_TOPIC_PING)){
    client.publish(MQTT_TOPIC_PINGR, " ");
  }

  // If we get a switch request then respond to it
  if (strstr(topic,MQTT_TOPIC_SWITCH)){
    if (msgString.equals("1")) {
      newState = STATE_FOGGING;
    }
    else {
      newState = STATE_CHARGING;
    }
  }

  // If we get a pulse request then respond to it
  if (strstr(topic,MQTT_TOPIC_PULSE)){
    timerSec = msgString.toInt();
    if (timerSec>maxFogTimeSec) {
      timerSec = maxFogTimeSec;
    }
    if (timerSec>0) {
      isPulseCommand = true;
    }
    newState = STATE_FOGGING;
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
      // ... and resubscribe
      client.subscribe(MQTT_TOPIC_PING);
      client.subscribe(MQTT_TOPIC_SWITCH);
      client.subscribe(MQTT_TOPIC_PULSE);
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

  pinMode(RELAY_PIN, OUTPUT);
  digitalWrite(RELAY_PIN, HIGH);
}

void loop() {

  if (!client.connected()) {
    reconnect();
  }

  // Look for any new messages
  client.loop();

  // Process any state transitions
  if (currentState!=newState) {
    
    Serial.print("Current State: ");
    printState(currentState);
    Serial.print(" New State: ");
    printState(newState);
    Serial.println("");
    
    if (newState == STATE_UNKNOWN) {
      
    }
    else if (newState == STATE_STARTUP) {
      // Do any startup code here
      currentState = newState;
      newState = STATE_IDLE;
    }
    else if (newState == STATE_IDLE) {
      // Reset any variables because we have dropped into the idle state
      if (currentState == STATE_CHARGING) {
        client.publish(MQTT_TOPIC_CHARGE,"0");
      }
      currentState = newState;
    }
    else if (newState == STATE_FOGGING) {
      // Check to see if we can transition into this state 
      if (currentState == STATE_IDLE){
        digitalWrite(BUILTIN_LED, LOW);
        digitalWrite(RELAY_PIN, LOW);
        Serial.println("Fog On");
        client.publish(MQTT_TOPIC_ON, "1");

        // Start the fogging timer
        if (isPulseCommand) {
          interval = timerSec * 1000;
        }
        else {
          interval = maxFogTimeSec * 1000;
        }
        timedOut = false;
        timer = millis();
        currentState = newState;
      }
      else {
        Serial.println("Fogging not allowed");
        newState = currentState;
      }
    }
    else if (newState == STATE_CHARGING) {
      if (currentState == STATE_FOGGING){
        digitalWrite(BUILTIN_LED, HIGH);
        digitalWrite(RELAY_PIN, HIGH);
        Serial.println("Fog Off");
        client.publish(MQTT_TOPIC_ON, "0");
        client.publish(MQTT_TOPIC_CHARGE,"1");

        // Start the charging timer
        interval = maxChargeTimeSec * 1000;
        timedOut = false;
        timer = millis();
        currentState = newState;
      }
      else {
        Serial.println("Charging not allowed");
        newState = currentState;
      }
    }
  }
  

  // Check to see if the timer has reached the end
  if ((!timedOut) && ((millis() - timer) > interval)) {
    timedOut = true;
    Serial.println("Timeout reached");
    if (currentState == STATE_FOGGING) {
      newState = STATE_CHARGING;
    }
    else if (currentState == STATE_CHARGING) {
      newState = STATE_IDLE;
    }
  }
}


