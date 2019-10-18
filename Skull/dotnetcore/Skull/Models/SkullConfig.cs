﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Skull
{
    public class SkullConfig
    {
        /// <summary>
        /// The max value of the servo to make the head look up 100%
        /// </summary>
        public int HeadYMax { get; set; }

        /// <summary>
        /// The min value of the servo to make the head look down -100%
        /// </summary>
        public int HeadYMin { get; set; }

        /// <summary>
        /// The servo deafult value 
        /// </summary>
        public int HeadYDefault { get; set; }

        /// <summary>
        /// The servo value to make the head look left 100% 
        /// </summary>
        public int HeadXMin { get; set; }

        /// <summary>
        /// The servo value to make the head look right 100% 
        /// </summary>
        public int HeadXMax { get; set; }

        /// <summary>
        /// The servo value to make the head look straight ahead 
        /// </summary>
        public int HeadXDefault { get; set; }

        /// <summary>
        /// Servo Value for opening the jaw all the way
        /// </summary>
        public int JawMax { get; set; }

        /// <summary>
        /// Servo value for closing the jaw all the way
        /// </summary>
        public int JawMin { get; set; }

        /// <summary>
        /// Servo default value
        /// </summary>
        public int JawDefault { get; set; }

        /// <summary>
        /// The PWM Channel that the Jaw Servo is on
        /// </summary>
        public int JawChannel { get; set; }

        /// <summary>
        /// The PWM Channel that the Head X Servo is on
        /// </summary>
        public int HeadXChannel { get; set; }

        /// <summary>
        /// The PWM Channel that the Head Y Servo is on
        /// </summary>
        public int HeadYChannel { get; set; }

        /// <summary>
        /// Determines which driver will be used for the server motors
        /// </summary>
        public string ServoMotorDriver { get; set; }
    }
}
