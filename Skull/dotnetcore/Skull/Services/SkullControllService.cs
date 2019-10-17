
using Iot.Device.Pwm;
using Iot.Device.ServoMotor;
using Microsoft.Extensions.Logging;
using System;
using System.Device.I2c;
using System.Device.Pwm;
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
        private Pca9685 _pwmDevice;
        private ServoMotor _jaw;
        private PwmChannel _jawChannel;
        private ServoMotor _headX;
        private ServoMotor _headY;

        public SkullControlService(ILogger<SkullControlService> logger, SkullConfig skullConfig, CommandQueue commandQueue)
        {
            _logger = logger;
            _skullConfig = skullConfig;
            _commandQueue = commandQueue;
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
            _logger.LogDebug("SetHeadX({0})", percent);
            if (percent < -100 || percent > 100)
            {
                _logger.LogWarning("Invalid percentage value for the Head X {0}", percent);
                return;
            }
            int mappedValue = percent.Map(-100, 100, 0, 100);
            _pwmDevice.SetDutyCycle(_skullConfig.HeadXChannel, mappedValue/100);

        }

        public void SetHeadY(int percent)
        {
            _logger.LogDebug("SetHeadY({0})", percent);
            if (percent < -100 || percent > 100)
            {
                _logger.LogWarning("Invalid percentage value for the Head Y {0}", percent);
                return;
            }
            int mappedValue = percent.Map(-100, 100, 0, 100);
            _pwmDevice.SetDutyCycle(_skullConfig.HeadYChannel, mappedValue / 100);

        }

        public void Bow()
        {
            _logger.LogDebug("Bow()");
            SetHeadX(-100);
            SetHeadY(0);

        }

        public void JawOpen()
        {
            _logger.LogDebug("JawOpen()");
            SetJaw(100);

        }

        public void JawClose()
        {
            _logger.LogDebug("JawClose()");
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

            var busId = 1;
            var selectedI2cAddress = 00000000;     // A5 A4 A3 A2 A1 A0
            var deviceAddress = Pca9685.I2cAddressBase + selectedI2cAddress;

            var settings = new I2cConnectionSettings(busId, deviceAddress);
            var device = I2cDevice.Create(settings);
            _pwmDevice = new Pca9685(device);
            _pwmDevice.PwmFrequency = 50;
            //_pwmDevice.SetDutyCycleAllChannels(0.5);
            _jawChannel = _pwmDevice.CreatePwmChannel(_skullConfig.JawChannel);
            _jawChannel.DutyCycle = 0.5;
            _jaw = new ServoMotor(_jawChannel, 180, 700, 2200);
            _jaw.Start();
            _jaw.WriteAngle(_skullConfig.JawMin);

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
            if (_pwmDevice!=null)
            {
                _jaw.Stop();
                _jawChannel.Stop();
                _jaw.Dispose();
                _jawChannel.Dispose();
                _pwmDevice.Dispose();
                _pwmDevice = null;
            }
            if (_tokenSource != null)
                _tokenSource.Cancel();
        }

    }
}