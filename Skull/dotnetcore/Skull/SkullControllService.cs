
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Skull
{
    public class SkullControlService
    {
        private readonly ILogger _logger;
        private readonly SkullConfig _skullConfig;
        private readonly CommandQueue _commandQueue;
        private CancellationTokenSource _tokenSource;

        public SkullControlService(ILogger<SkullControlService> logger, SkullConfig skullConfig, CommandQueue commandQueue)
        {
            _logger = logger;
            _skullConfig = skullConfig;
            _commandQueue = commandQueue;
        }

        public void SetJaw(int percent)
        {
        }

        public void SetHeadX(int percent)
        {

        }

        public void SetHeadY(int percent)
        {

        }

        public void Bow()
        {

        }

        public void JawOpen()
        {

        }

        public void JawClose()
        {

        }

        public void Laugh()
        {

        }

        /// <summary>
        /// Jaw laughing with a delay before starting (in milliseconds) and open/close jaw how many times
        /// </summary>
        /// <param name="startDelay"></param>
        /// <param name="howManyTimes"></param>
        public void LaughWithDelay(int startDelay, int howManyTimes)
        {
            Thread.Sleep(startDelay);
            for (int i = 0; i < howManyTimes; i++)
            {
                JawOpen();
                Thread.Sleep(100);
                JawClose();
                Thread.Sleep(100);
            }

        }

        public void Connect()
        {
            _logger.LogInformation("Connecting to the servo controller");

            _tokenSource = new CancellationTokenSource();
            var ct = _tokenSource.Token;

            var task = Task.Run(() =>
            {
                // Were we already canceled?
                ct.ThrowIfCancellationRequested();

                bool moreToDo = true;
                while (moreToDo)
                {
                    var command = _commandQueue.Dequeue();
                    if (command!=null)
                    {
                        _logger.LogInformation("Command id: {0}", command.CommandId);
                    }
                    // Poll on this property if you have to do
                    // other cleanup before throwing.
                    if (ct.IsCancellationRequested)
                    {
                        // Clean up here, then...
                        ct.ThrowIfCancellationRequested();
                    }
                    Thread.Sleep(100);
                }
            }, _tokenSource.Token); 
        }

        public void Disconnect()
        {
            _logger.LogInformation("Disconnecting from the servo controller");
            if (_tokenSource != null)
                _tokenSource.Cancel();
        }

    }
}