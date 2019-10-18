using Iot.Device.Pwm;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Device.Pwm;
using System.Text;

namespace Skull.HAL
{
    public class PwmI2CDriver : IPwmDriver
    {
        private readonly ILogger _logger;
        private Pca9685 _pwmDevice;
        private int _busId = 1;
        private int _selectedI2CDevice = 0;

        public PwmI2CDriver(ILogger<PwmI2CDriver> logger)
        {
            _logger = logger;
        }

        public int BusId 
        { 
            get { return _busId; }
            set
            {
                if (_busId != value)
                    _busId = value;
            }
        }

        public int SelectedDeviceAddress
        {
            get { return _selectedI2CDevice; }
            set
            {
                if (_selectedI2CDevice != value)
                    _selectedI2CDevice = value;
            }
        }

        public bool Connect()
        {
            _logger.LogInformation("Connecting to the Pwm I2C Driver");

            var deviceAddress = Pca9685.I2cAddressBase + _selectedI2CDevice;

            var settings = new I2cConnectionSettings(_busId, deviceAddress);
            var device = I2cDevice.Create(settings);
            _pwmDevice = new Pca9685(device);
            _pwmDevice.SetDutyCycleAllChannels(0.5)
;
            return true;
        }

        public bool Disconnect()
        {
            
            return true;
        }

        public PwmChannel BuildChannel(int channel)
        {
            return _pwmDevice.CreatePwmChannel(channel);
        }
    }
}
