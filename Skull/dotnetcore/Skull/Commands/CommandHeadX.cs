using System;
using System.Collections.Generic;
using System.Text;

namespace Skull
{
    public class CommandHeadX : CommandBase
    {
        public CommandHeadX()
        {
            this.CommandId = CommandIdEnum.HeadX;
        }

        public int Value { get; set; }
    }
}
