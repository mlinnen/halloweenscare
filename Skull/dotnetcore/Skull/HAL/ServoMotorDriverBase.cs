using Iot.Device.ServoMotor;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.Pwm;
using System.Text;

namespace Skull.HAL
{
    public class ServoMotorDriverBase
    {
        protected  ILogger _logger;

        protected int _id;
        protected string _name;
        protected int _minAngle = 0;
        protected int _maxAngle = 180;
        protected int _defaultAngle = 0;
        protected int _currentAngle;

        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                }
            }
        }

        public int MinAngle
        {
            get { return _minAngle; }
            set
            {
                if (_minAngle != value)
                {
                    if (ValidateAngle(value))
                        _minAngle = value;
                }
            }
        }

        public int MaxAngle
        {
            get { return _maxAngle; }
            set
            {
                if (_maxAngle != value)
                {
                    if (ValidateAngle(value))
                        _maxAngle = value;
                }
            }
        }

        public int DefaultAngle
        {
            get { return _defaultAngle; }
            set
            {
                if (_defaultAngle != value)
                {
                    if (ValidateAngle(value))
                        _defaultAngle = value;
                }
            }
        }

        public int CurrentAngle
        {
            get { return _currentAngle; }
        }

        protected bool ValidateAngle(int value)
        {
            if (value >= _minAngle && value <= _maxAngle)
                return true;
            _logger.LogWarning("Motor {1} ({0}) Angle {2} exceeded the range of {3} to {4}", _id, _name, value, _minAngle, _maxAngle);
            return false;
        }

    }
}
