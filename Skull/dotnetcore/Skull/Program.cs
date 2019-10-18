﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skull.HAL;

namespace Skull
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    configHost.AddEnvironmentVariables(prefix: "HOST_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", 
                        optional: true);
                    configApp.AddEnvironmentVariables(prefix: "DOTNET_");
                    configApp.AddCommandLine(args);
                  
                })
                .ConfigureServices((hostContext, services) =>
                {
                    MqttConfig mqttConfig = new MqttConfig();
                    hostContext.Configuration.GetSection("MqttBroker").Bind(mqttConfig);
                    services.AddSingleton<MqttConfig>(mqttConfig);

                    SkullConfig skullConfig = new SkullConfig();
                    hostContext.Configuration.GetSection("SkullConfig").Bind(skullConfig);
                    services.AddSingleton<SkullConfig>(skullConfig);

                    services.AddHostedService<LifetimeEventsHostedService>();
                    services.AddSingleton<SkullControlService>();
                    services.AddSingleton<SkullMqttService>();
                    if (string.IsNullOrEmpty(skullConfig.ServoMotorDriver) || skullConfig.ServoMotorDriver.Equals("pca986", StringComparison.CurrentCultureIgnoreCase))
                    {
                        services.AddSingleton<IPwmDriver, PwmI2CDriver>();
                        services.AddTransient<IServoMotor, ServoMotorDriver>();
                    }
                    else if (skullConfig.ServoMotorDriver.Equals("mock", StringComparison.CurrentCultureIgnoreCase))
                    {
                        services.AddSingleton<IPwmDriver, PwmMockDriver>();
                        services.AddTransient<IServoMotor, ServoMotorMockDriver>();
                    }
                    services.AddSingleton<CommandQueue>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();        
        }
    }
}
