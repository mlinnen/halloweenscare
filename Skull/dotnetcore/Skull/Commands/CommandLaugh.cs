using System;
using System.Collections.Generic;
using System.Text;

namespace Skull
{
    public class CommandLaugh : CommandBase
    {
        public CommandLaugh()
        {
            this.CommandId = CommandIdEnum.Laugh;
        }

        public int StartDelay { get; set; }

        public int NumberOfTimes { get; set; }
    }
}
