using Iot.Device.Pwm;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Device.Pwm;
using System.Text;

namespace Skull.HAL
{
    public class PwmMockDriver : IPwmDriver
    {
        private readonly ILogger _logger;

        public PwmMockDriver(ILogger<PwmI2CDriver> logger)
        {
            _logger = logger;
        }

        public bool Connect()
        {
            _logger.LogInformation("Connecting to the Pwm Mock Driver");

            return true;
        }

        public bool Disconnect()
        {
            
            return true;
        }

        public PwmChannel BuildChannel(int channel)
        {
            return null;
        }
    }
}
