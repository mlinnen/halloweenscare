using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Skull
{
    internal class LifetimeEventsHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly SkullMqttService _skullMqttService;
        private readonly SkullControlService _skullControlService;

        public LifetimeEventsHostedService(
            ILogger<LifetimeEventsHostedService> logger, 
            IHostApplicationLifetime appLifetime,
            SkullMqttService mqttService,
            SkullControlService skullControlService)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _skullMqttService = mqttService;
            _skullControlService = skullControlService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");

            // Perform post-startup activities here
            _skullMqttService.Connect();
            _skullControlService.Connect();
            
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");

            // Perform on-stopping activities here
            _skullMqttService.Disconnect();
            _skullControlService.Disconnect();
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");

            // Perform post-stopped activities here
        }
    }
}