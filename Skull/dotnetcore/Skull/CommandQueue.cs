using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Skull
{
    public class CommandQueue
    {
        private ConcurrentQueue<CommandBase> _commandQueue = new ConcurrentQueue<CommandBase>();

        public void Enqueue(CommandBase command)
        {
            _commandQueue.Enqueue(command);
        }

        public CommandBase Dequeue()
        {
            CommandBase command;
            if (_commandQueue.TryDequeue(out command))
                return command;
            return null;
        }
    }

}
