# Skull
The idea behind this project is to have an animated skull head that is controlled by MQTT messages.  You can use any device you want 
to actually perform the animation and communicate via MQTT as long as you adhere to the following specifications: 

* The device supports configuring a **root** topic.  This will allow all commands to the device to be prefixed.

## MQTT Topics and Messages

**{root}** is your root topic that you want to use to isolate the device on the message bus. 
**{id}** is the Skull Id you want to use to uniquely identify multiple Skulls on the bus.  This **{id}** can be a number or characters as long as it is a set of MQTT Topic valid characters.

### MQTT Subscriptions
The following subscription topics should be supported by the device.  

Ask what Skulls are online.  
```
/{root}/skull/ping
```

Tell the head to move up or down based on a percentage from the level position.  A value of 0 for the heady means the head is looking straight ahead from an up/down perspective.  A value of 100 means to lift the head up 100% as far as it can go.  A value of -100 means to bow the head down 100%.
  
```
/{root}/skull/heady/{id} 100
```

Tell the head to move left or right based on a percentage from the level position.  A value of 0 for the headx means the head is looking straight ahead from a left/right perspective.  A value of 100 means to rotate the head left 100% as far as it can go.  A value of -100 means to rotate the head right 100%.

```
/{root}/skull/headx/{id} 100
```

Tell the jaw to open 0 to 100%.  A value of 0 means close the mouth.  A value of 100 means to open the mouth as far as it can.

```
/{root}/skull/jaw/{id} 100
```

Tell the skull jaw to do the laugh animation.  If no payload is sent to the topic then the laugh will start immediately and continue as long as the device decides to continue the laugh. Two optional parameters will change when the laugh starts and how many times the jaw opens and closes.

```
/{root}/skull/laugh/{id}
```

The following laugh will start after 1000 milliseconds and the jaw will open and close 10 times.
 
```
/{root}/skull/laugh/{id} 1000 10
```

Tell the skull to do the bow animation

```
/{root}/skull/bow/{id}
```

Tell the skull to do the yes animation

```
/{root}/skull/yes/{id}
```

Tell the skull to do the no animation

```
/{root}/skull/no/{id}
```

### MQTT Publications
The following topics and messages are published by this device when it's state has changed.

A skulls response to the ping request.
```
/{root}/skull/pingr/{id} 
```

## Implementations 
* [ESP8266](ESP8266/)