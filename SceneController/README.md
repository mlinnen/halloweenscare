# Scene Controller
The idea for this device is to provide the brains behind listening to sensor data and making decisions on what should happen to a scene.  There really isn't any specifications for this device as it is really about tying the devices together into a workflow.   

## Motion Play Next
This is a pretty simple scene controller.  It basically listens for motion sensor data and when motion is detected it will command a media player to play the next media item in a sequence.
* [Motion Play Next Media Item on a Raspberry Pi using Python](MotionPlayNext/RaspberryPi/)
* Motion Play Next Media Item on a Raspberry Pi using Node-Red (Coming Soon)   


### Raspberry Pi
#### Python
MotionPlayNext.py is a simple controller that listens for motion from any of the motion detectors and tells the media player id=1 to play it's next media item. 

#### Node-Red (Coming Soon)