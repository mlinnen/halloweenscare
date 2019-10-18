using System;
using System.Collections.Generic;
using System.Device.Pwm;
using System.Text;

namespace Skull.HAL
{
    public interface IPwmDriver
    {
        bool Connect();
        bool Disconnect();
        PwmChannel BuildChannel(int channel);
    }
}
