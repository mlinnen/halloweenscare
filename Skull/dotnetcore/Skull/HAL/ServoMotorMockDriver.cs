using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skull.HAL
{
    public class ServoMotorMockDriver : ServoMotorDriverBase, IServoMotor
    {

        public ServoMotorMockDriver(ILogger<ServoMotorMockDriver> logger)
        {
            _logger = logger;
        }

        public bool Connect()
        {
            _logger.LogInformation("Connecting to mock servo motor: {0} {1}", _id, _name);
            _logger.LogInformation("Mock Servo Motor Min: {0} Max: Default: angles", _minAngle, _maxAngle, _defaultAngle);
            return true;
        }

        public bool Disconnect()
        {
            _logger.LogInformation("Disconnecting from mock servo motor: {0} {1}", _id, _name);
            return true;
        }

        public void WriteAngle(int angle)
        {
            _logger.LogDebug("Write angle {0} to mock motor {1} {2}", angle, _id, _name);

            if (ValidateAngle(angle))
            {
                _logger.LogDebug("New angle {0} for mock motor {1} {2}", angle, _id, _name);
                _currentAngle = angle;
            }

        }
    }
}
