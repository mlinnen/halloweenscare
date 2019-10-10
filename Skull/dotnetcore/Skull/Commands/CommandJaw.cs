using System;
using System.Collections.Generic;
using System.Text;

namespace Skull
{
    public class CommandJaw:CommandBase
    {
        public CommandJaw()
        {
            this.CommandId = CommandIdEnum.Jaw;
        }

        public int Value { get; set; }
    }
}
