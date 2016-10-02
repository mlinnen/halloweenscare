# Play Media
The idea behind this project is to have a stand alone device that can be controlled to play a video or sound using MQTT messaging.  You can use any device you want to actually play the video or sounds as long as you adhere to the following specifications: 

* The device must have a way to access the video or sounds.  This might mean the device stores them in it's own storage mechanism or if it can support streaming content then that is fine too. 
* The media is played in its full length unless a **stop** command is issued.  Subsequent **play** or **playnext** commands will be ignored while media is being played.
* The device supports configuring a **root** topic.  This will allow all commands to the device to be prefixed.

## MQTT Topics and Messages

**{root}** is your root topic that you want to use to issolate the device on the message bus. 
**{id}** is the Media Player Id you want to use to uniquely control multiple Media Players on the bus.  This **{id}** can be a number or characters as long as it is a set of MQTT Topic valid characters.

### MQTT Subscriptions
The following subscription topics should be supported by the device.  

Play media item.  The message body should contain an integer >0 that is an index into which media item should be played.  This example below is requesting media item 1 to be played.
```
/{root}/media/play/{id} 1
```

Play the next media item in rotation if the end of the rotation has been reached then cycle back to first media item in the rotation. 
```
/{root}/media/playnext/{id}
```

Stop any existing media item from playing.
```
/{root}/media/stop/{id}
```

Stop any existing media item from playing.  Used to stop multiple media players from playing content without having to address each one individually.
```
/{root}/media/stopall
```

Ask the media player how many media items it has available for play.
```
/{root}/media/items/{id}
```

Ask what media players are online.  
```
/{root}/media/ping
```

### MQTT Publications
The following topics and messages are published by this device when it's state has changed.

In response to a **play** or **playnext** command this topic and message will be published when the media item has been started.  The number represents the
index of the media item.
```
/{root}/media/playstarted/{id} 1
```

This topic and message will be published when the media item ends or is stopped by the **stop** command. The number represents the
index of the media item that was stopped or ended.
```
/{root}/media/playended/{id} 1
```

A media players response to the number of media items it has available for play. In this example the media player has 5 items.
```
/{root}/media/itemsr/{id} 5
```

A media players response to the ping request.
```
/{root}/media/pingr/{id} 
```

