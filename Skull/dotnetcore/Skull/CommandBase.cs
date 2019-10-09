using System;
using System.Collections.Generic;
using System.Text;

namespace Skull
{
    public class CommandBase
    {
        private CommandIdEnum _commandId = CommandIdEnum.Unknown;
        public CommandIdEnum CommandId
        {
            get { return _commandId; }
            set
            {
                if (_commandId!=value)
                {
                    _commandId = value;
                }
            }
        }
    }
}
