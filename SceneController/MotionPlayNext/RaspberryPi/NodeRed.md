
# Scene Controller 
# Motion Play Next using Node-Red on a Raspberry Pi
[Node-Red](http://nodered.org/) comes already installed on Raspian Jessie for the Raspberry Pi.  The following instructions assume you are using the version already installed on the Raspberry Pi.  These steps also assume the following:
* You already have an MQTT broker setup on your network.
* You already have a Motion Detector device running on your network.
* You already have a Play Media device running on your network
* You have set your **{root}** topic to **halloween** for the Motion Detector and Media Player

You need to know the IP address of your Raspberry Pi in order to do a lot of the steps highlighted below.

1. Open a terminal window on your Raspberry Pi and execute **node-red-start**
    * Check out the [Node-Red Raspberry Pi Documentation](http://nodered.org/docs/hardware/raspberrypi) to configure Node-Red to run on boot.
2. Open up a browser and go to http://[your raspberry pi IP]:1880
3. Add an **Input -> mqtt** node onto the canvas
4. Double click the node to edit the properties.  If you have not setup an MQTT Server yet in Node-Red you will have to use the pencil edit icon to do so now.
5. Set the mqtt Topic to **halloween/motion/value/+**
    * Note: the **+** represents match any of the motion detectors. 
    * Note: change to some other root topic if your Motion Detector device did not use halloween. 
6. Set the QoS to **2**
7. Name the node **Any Motion Change**
8. Click the **Done** button
9. Add a **Function -> function** node to the canvas
10. Edit the function by double clicking it.
11. Add the following code to the function

    ``` javascript
    if (msg.payload=='1')
        return msg;
    return null;
    ```
    
12. Name the function **If Motion Detected**
13. Click the **Done** button
14. Wire the output of the **Any Motion Change** to the input of **If Motion Detected** by dragging a line between the two.
15. Add an **Output -> mqtt** node to the canvas
16. Edit the node by double clicking it.
17. Set the Topic to **halloween/media/playnext/1**
    * Note: change to some other root topic if your Media Player device did not use halloween. 
18. Set the QoS to 1
19. Click the **Done** button
20. Wire up the output of the **If Motion Detected** node to the input of the **halloween/media/playnext/1** node.
20. Deploy the flow to node-red by clicking the **Deploy** button

When you are done your node-red flow should look like the following:

![Node-Red Motion Play Next Flow](img/noderedmotionplaynext.png)

Activate the Motion sensor and you should see/hear one of the media items playing on your Media Player.




