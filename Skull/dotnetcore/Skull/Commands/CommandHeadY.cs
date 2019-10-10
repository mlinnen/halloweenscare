using System;
using System.Collections.Generic;
using System.Text;

namespace Skull
{
    public class CommandHeadY : CommandBase
    {
        public CommandHeadY()
        {
            this.CommandId = CommandIdEnum.HeadY;
        }

        public int Value { get; set; }
    }
}
