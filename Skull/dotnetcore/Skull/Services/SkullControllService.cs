
using Microsoft.Extensions.Logging;
using Skull.HAL;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skull
{
    public class SkullControlService
    {
        private readonly ILogger _logger;
        private readonly SkullConfig _skullConfig;
        private readonly CommandQueue _commandQueue;
        private CancellationTokenSource _tokenSource;
        private IPwmDriver _pwmDriver;
        private IServoMotor _jaw;
        private IServoMotor _headX;
        private IServoMotor _headY;

        public SkullControlService(ILogger<SkullControlService> logger,IPwmDriver pwmDriver, SkullConfig skullConfig, CommandQueue commandQueue, IServoMotor jaw, IServoMotor headX, IServoMotor headY)
        {
            _logger = logger;
            _skullConfig = skullConfig;
            _commandQueue = commandQueue;
            _pwmDriver = pwmDriver;
            _jaw = jaw;
            _headX = headX;
            _headY = headY;
        }

        /// <summary>
        /// 0 to 100, 0 = close jaw, 100 = open jaw.
        /// </summary>
        /// <param name="percent"></param>
        public void SetJaw(int percent)
        {
            _logger.LogInformation("SetJaw({0})", percent);
            if (percent<0 || percent>100)
            {
                _logger.LogWarning("Invalid percentage value for the jaw {0}", percent);
                return;
            }
            _logger.LogInformation("jaw min angle {0}", _skullConfig.JawMin);
            _logger.LogInformation("jaw max angle {0}", _skullConfig.JawMax);
            var angle = percent.Map(0, 100, _skullConfig.JawMin, _skullConfig.JawMax);
            _logger.LogInformation("SetJaw angle {0}", angle);
            _jaw.WriteAngle(angle);
        }

        public void SetHeadX(int percent)
        {
            _logger.LogInformation("SetHeadX({0})", percent);
            if (percent < -100 || percent > 100)
            {
                _logger.LogWarning("Invalid percentage value for the Head X {0}", percent);
                return;
            }
            int mappedValue = percent.Map(-100, 100, 0, 100);
            _headX.WriteAngle(mappedValue);
        }

        public void SetHeadY(int percent)
        {
            _logger.LogInformation("SetHeadY({0})", percent);
            if (percent < -100 || percent > 100)
            {
                _logger.LogWarning("Invalid percentage value for the Head Y {0}", percent);
                return;
            }
            int mappedValue = percent.Map(-100, 100, 0, 100);
            _headY.WriteAngle(mappedValue);
        }

        public void Bow()
        {
            _logger.LogInformation("Bow()");
            SetHeadX(-100);
            SetHeadY(0);

        }

        public void JawOpen()
        {
            _logger.LogInformation("JawOpen()");
            SetJaw(100);

        }

        public void JawClose()
        {
            _logger.LogInformation("JawClose()");
            SetJaw(0);
        }

        public void Laugh()
        {
            LaughWithDelay(0, 1);
        }

        /// <summary>
        /// Jaw laughing with a delay before starting (in milliseconds) and open/close jaw how many times
        /// </summary>
        /// <param name="startDelay"></param>
        /// <param name="howManyTimes"></param>
        public void LaughWithDelay(int startDelay, int howManyTimes)
        {
            _logger.LogInformation("Laugh with delay {0} times {1}", startDelay, howManyTimes);
            if (startDelay!=0)
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

            _pwmDriver.Connect();

            _jaw.Id = _skullConfig.JawChannel;
            _jaw.Name = "Jaw";
            _jaw.MaxAngle = _skullConfig.JawMax;
            _jaw.MinAngle = _skullConfig.JawMin;
            _jaw.DefaultAngle = _skullConfig.JawMin;

            _headX.Id = _skullConfig.HeadXChannel;
            _headX.Name = "Head X";
            _headX.MaxAngle = _skullConfig.HeadXMax;
            _headX.MinAngle = _skullConfig.HeadXMin;
            _headX.DefaultAngle = _skullConfig.HeadYDefault;

            _headY.Id = _skullConfig.HeadYChannel;
            _headY.Name = "Head Y";
            _headY.MaxAngle = _skullConfig.HeadYMax;
            _headY.MinAngle = _skullConfig.HeadYMin;
            _headY.DefaultAngle = _skullConfig.HeadYDefault;

            _jaw.Connect();
            _headX.Connect();
            _headY.Connect();

            _tokenSource = new CancellationTokenSource();
            var ct = _tokenSource.Token;

            var task = Task.Run(() =>
            {
                // Were we already canceled?
                ct.ThrowIfCancellationRequested();

                bool moreToDo = true;
                while (moreToDo)
                {
                    try
                    {
                        var command = _commandQueue.Dequeue();
                        bool processedCommand = false;
                        if (command != null)
                        {
                            switch (command.CommandId)
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
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing the command from the bus");
                    }
                }
            }, _tokenSource.Token); 
        }

        public void Disconnect()
        {
            _logger.LogInformation("Disconnecting from the servo controller");
            if (_pwmDriver!=null)
            {
                _pwmDriver.Disconnect();
                _jaw.Disconnect();
                _headX.Disconnect();
                _headY.Disconnect();
            }
            if (_tokenSource != null)
                _tokenSource.Cancel();
        }

    }
}