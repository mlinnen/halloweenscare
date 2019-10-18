using System;
using System.Collections.Generic;
using System.Text;

namespace Skull.HAL
{
    public interface IServoMotor
    {
        int Id { get; set; }
        string Name { get; set; }
        int MinAngle { get; set; }
        int MaxAngle { get; set; }
        int DefaultAngle { get; set; }
        int CurrentAngle { get; }
        bool Connect();
        bool Disconnect();
        void WriteAngle(int angle);
    }
}
