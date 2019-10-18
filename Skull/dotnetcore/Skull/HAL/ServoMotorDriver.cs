using Iot.Device.ServoMotor;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.Pwm;
using System.Text;

namespace Skull.HAL
{
    public class ServoMotorDriver : ServoMotorDriverBase,IServoMotor
    {
        private readonly IPwmDriver _pwmDriver;
        private ServoMotor _servoMotor;

        public ServoMotorDriver(ILogger<ServoMotorDriver> logger, IPwmDriver pwmDriver)
        {
            _logger = logger;
            _pwmDriver = pwmDriver;
        }

        public bool Connect()
        {
            _logger.LogInformation("Connecting to servo motor: {0} {1}", _id, _name);
            _logger.LogInformation("Servo Motor Min: {0} Max: Default: angles", _minAngle,_maxAngle,_defaultAngle);

            var channel = _pwmDriver.BuildChannel(Id);
            channel.DutyCycle = 0.5;

            _servoMotor = new ServoMotor(channel, _maxAngle, 700, 2200);
            _servoMotor.Start();
            _servoMotor.WriteAngle(_defaultAngle);
            _currentAngle = _defaultAngle;

            return true;
        }

        public bool Disconnect()
        {
            _logger.LogInformation("Disconnecting from servo motor: {0} {1}", _id, _name);
            _servoMotor.Stop();
            _servoMotor.Dispose();
            _servoMotor = null;
            return true;
        }

        public void WriteAngle(int angle)
        {
            _logger.LogDebug("Write angle {0} to motor {1} {2}",angle,_id, _name);

            if (ValidateAngle(angle))
            {
                _servoMotor.WriteAngle(angle);
                _currentAngle = angle;
            }
        }

    }
}
