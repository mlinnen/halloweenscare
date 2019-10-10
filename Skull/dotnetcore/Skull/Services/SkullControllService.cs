
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
            _logger.LogDebug("SetJaw({0})", percent);
        }

        public void SetHeadX(int percent)
        {
            _logger.LogDebug("SetHeadX({0})", percent);

        }

        public void SetHeadY(int percent)
        {
            _logger.LogDebug("SetHeadY({0})", percent);

        }

        public void Bow()
        {
            _logger.LogDebug("Bow()");

        }

        public void JawOpen()
        {
            _logger.LogDebug("JawOpen()");

        }

        public void JawClose()
        {
            _logger.LogDebug("JawClose()");

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

        public void Yes()
        {

        }

        public void No()
        {

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
                    bool processedCommand = false;
                    if (command!=null)
                    {
                        switch(command.CommandId)
                        {
                            case CommandIdEnum.Bow:
                                Bow();
                                break;
                            case CommandIdEnum.HeadX:
                                this.SetHeadX(((CommandHeadX)command).Value);
                                break;
                            case CommandIdEnum.HeadY:
                                this.SetHeadY(((CommandHeadY)command).Value);
                                break;
                            case CommandIdEnum.Jaw:
                                this.SetJaw(((CommandJaw)command).Value);
                                break;
                            case CommandIdEnum.Laugh:
                                this.LaughWithDelay(((CommandLaugh)command).StartDelay, ((CommandLaugh)command).NumberOfTimes);
                                break;
                            case CommandIdEnum.Yes:
                                this.Yes();
                                break;
                            case CommandIdEnum.No:
                                this.No();
                                break;
                            default:
                                _logger.LogWarning("Unknown Command id: {0}", command.CommandId);
                                break;
                        }
                        processedCommand = true;
                    }
                    // Poll on this property if you have to do
                    // other cleanup before throwing.
                    if (ct.IsCancellationRequested)
                    {
                        // Clean up here, then...
                        ct.ThrowIfCancellationRequested();
                    }
                    if (!processedCommand)
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