using System;
using System.Collections.Generic;
using System.Text;

namespace Skull
{
    public class CommandBow: CommandBase
    {
        public CommandBow()
        {
            this.CommandId = CommandIdEnum.Bow;
        }
    }
}
