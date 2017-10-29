#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>
#include "mywifi.h"
#include "mymqttbroker.h"
#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#include "ctype.h"

const char* ssid = WLAN_SSID;
const char* password = WLAN_PASS;
const char* mqtt_server = MQTT_SERVER;

const char* MQTT_TOPIC_PING = "/halloween/skull/ping";
const char* MQTT_TOPIC_PINGR = "/halloween/skull/pingr/1";
const char* MQTT_TOPIC_HEAD_Y = "/halloween/skull/heady/1";
const char* MQTT_TOPIC_HEAD_X = "/halloween/skull/headx/1";
const char* MQTT_TOPIC_JAW = "/halloween/skull/jaw/1";
const char* MQTT_TOPIC_LAUGH = "/halloween/skull/laugh/1";
const char* MQTT_TOPIC_BOW = "/halloween/skull/bow/1";


// called this way, it uses the default address 0x40
Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver();
// you can also call it with a different address you want
//Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver(0x41);

// Depending on your servo make, the pulse width min and max may vary, you 
// want these to be as small/large as possible without hitting the hard stop
// for max range. You'll have to tweak them as necessary to match the servos you
// have!
#define SERVOMIN  40 // this is the 'minimum' pulse length count (out of 4096)
#define SERVOMAX  600 // this is the 'maximum' pulse length count (out of 4096)

WiFiClient espClient;
PubSubClient client(espClient);
char msg[50];
char message_buff[20];

#define HEAD_X_RIGHT  0 
#define HEAD_X_LEVEL  90 
#define HEAD_X_LEFT  180 
#define HEAD_Y_UP  30 
#define HEAD_Y_LEVEL  115 
#define HEAD_Y_DOWN  140 
#define JAW_CLOSE  50 
#define JAW_OPEN  110 

int xPercent = 0;
int yPercent = 0;
int jawPercent = 0;

// our servo
const uint8_t SERVO_HEAD_Y = 0;
const uint8_t SERVO_HEAD_X = 1;
const uint8_t SERVO_JAW = 2;

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
      client.subscribe(MQTT_TOPIC_HEAD_Y);
      client.subscribe(MQTT_TOPIC_HEAD_X);
      client.subscribe(MQTT_TOPIC_JAW);
      client.subscribe(MQTT_TOPIC_LAUGH);
      client.subscribe(MQTT_TOPIC_BOW);
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}

void setHeadX(uint8_t x)
{
  pwm.setPWM(SERVO_HEAD_X, 0, map(x,0,180,SERVOMIN,SERVOMAX));
}

void setHeadY(uint8_t y)
{
  pwm.setPWM(SERVO_HEAD_Y, 0, map(y,0,180,SERVOMIN,SERVOMAX));
}

void setHead(uint8_t y, uint8_t x)
{
  setHeadX(x);
  setHeadY(y);
}

void setJaw(uint8_t value)
{
  pwm.setPWM(SERVO_JAW, 0, map(value,0,180,SERVOMIN,SERVOMAX));
}

void setJawPercent(int percent)
{
  int jaw = 0;
  jaw = map(percent,0,100,JAW_CLOSE, JAW_OPEN);
  setJaw(jaw);
}

void setHeadXPercent(int percent)
{
  
  int x;
  if (percent<0)
  {
    x = map(percent,-100,0,HEAD_X_RIGHT, HEAD_X_LEVEL);
  }
  else if (percent>=0)
  {
    x = map(percent,0,100,HEAD_X_LEVEL,HEAD_X_LEFT);
  }
  Serial.print(" x=");
  Serial.print(x);
  Serial.println(" ");
  setHeadX(x);
}

void setHeadYPercent(int percent)
{
  Serial.print("%y=");
  Serial.print(percent);
  Serial.println(" ");
  int y;
  if (percent<0)
  {
    y = map(percent,-100,0,HEAD_Y_DOWN, HEAD_Y_LEVEL);
  }
  else if (percent>=0)
  {
    y = map(percent,0,100,HEAD_Y_LEVEL, HEAD_Y_UP);
  }
  Serial.print("y=");
  Serial.print(y);
  Serial.println(" ");
  setHeadY(y);
}

// PercentY = Head Up/Down
// PercentX = Head Left/Right
// PercentY = 0 to 100 Head Up
// PercentY = 0 to -100 Head Down
// PercentX = 0 to 100 Head Left
// PercentX = 0 to -100 Head Right
void setHeadPercent(int percenty, int percentx)
{
  setHeadYPercent(percenty);
  setHeadXPercent(percentx);
}

void level()
{
  setHeadPercent(0,0);
}

void lookLeft()
{
  setHeadXPercent(100);
}

void lookLeft(int percent)
{
  setHeadXPercent(percent);
}

void lookRight()
{
  setHeadXPercent(-100);
}

void lookRight(int percent)
{
  setHeadXPercent(percent);
}

void bow()
{
  setHeadXPercent(0);
  setHeadYPercent(-100);
}

void jawOpen()
{
  setJaw(JAW_OPEN);
}

void jawClose()
{
  setJaw(JAW_CLOSE);
}

void laugh()
{
  for(int i=0;i<4;i++)
  {
    jawOpen();
    delay(100);
    jawClose();
    delay(100);
  }
}

void laugh(int startDelay, int howManyTimes)
{
  delay(startDelay);
  for(int i=0;i<howManyTimes;i++)
  {
    jawOpen();
    delay(100);
    jawClose();
    delay(100);
  }
}

void test()
{
  Serial.println("bow");
  bow();
  delay(3000);
  Serial.println("level");
  level();
  delay(3000);
  laugh();
  delay(3000);
  Serial.println("Left");
  lookLeft();
  delay(3000);
  Serial.println("Right");
  lookRight();
  delay(3000);
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

  // If we get a bow request then respond to it
  if (strstr(topic,MQTT_TOPIC_BOW)){
    bow();
  }

  // If we get a laugh request then respond to it
  if (strstr(topic,MQTT_TOPIC_LAUGH)){
    if (msgString.length()>0)
    {
      String startDelayStr = getValue(msgString,' ',0);      
      String countStr = getValue(msgString,' ',1);      
      int delayMil = startDelayStr.toInt();
      int count = 4;
      if (countStr.length()>0)
      {
        count = countStr.toInt();
      }
      laugh(delayMil,count);
    }
    else
      laugh();
  }

  // If we get a heady request then respond to it
  if (strstr(topic,MQTT_TOPIC_HEAD_Y)){
    yPercent = msgString.toInt();
    setHeadYPercent(yPercent);
  }
  
  // If we get a headx request then respond to it
  if (strstr(topic,MQTT_TOPIC_HEAD_X)){
    xPercent = msgString.toInt();
    setHeadXPercent(xPercent);
  }
  
  // If we get a jaw request then respond to it
  if (strstr(topic,MQTT_TOPIC_JAW)){
    jawPercent = msgString.toInt();
    setJawPercent(jawPercent);
  }
}

void setup() {
  Serial.begin(9600);

  setup_wifi();
  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);

  pwm.begin();
  
  pwm.setPWMFreq(60);  // Analog servos run at ~60 Hz updates

  bow();

  yield();
}

void loop() {
  // test();
  if (!client.connected()) {
    reconnect();
  }

  // Look for any new messages
  client.loop();

}

String getValue(String data, char separator, int index)
{
  int found = 0;
  int strIndex[] = {0, -1};
  int maxIndex = data.length()-1;

  for(int i=0; i<=maxIndex && found<=index; i++){
    if(data.charAt(i)==separator || i==maxIndex){
        found++;
        strIndex[0] = strIndex[1]+1;
        strIndex[1] = (i == maxIndex) ? i+1 : i;
    }
  }

  return found>index ? data.substring(strIndex[0], strIndex[1]) : "";
}
