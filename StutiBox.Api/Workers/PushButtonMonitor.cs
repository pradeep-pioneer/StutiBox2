using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StutiBox.Api.Actors;
using StutiBox.Api.Models;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace StutiBox.Api.Workers
{
    public class PushButtonMonitor : IHostedService, IDisposable
    {
        private readonly IPlayerActor playerActor;
        private readonly IBassActor bassActor;
        private readonly ILogger<PushButtonMonitor> logger;
        private IGpioPin pin;
        private const string StartupFileName = "startup.mp3";
        private readonly string AppDirectory;
        private IShutdownActor _shutdownActor;

        public PushButtonMonitor(IPlayerActor playerActor, ILogger<PushButtonMonitor> logger, IBassActor bassActor, IShutdownActor shutdownActor)
        {
            this.playerActor = playerActor;
            this.logger = logger;
            this.bassActor = bassActor;
            this.AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _shutdownActor = shutdownActor;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.Log(LogLevel.Information, "Started monitor");
            var libraryItem = new LibraryItem(-1, Path.Combine(AppDirectory, StartupFileName), bassActor);
            if (playerActor.PlaybackState == PlaybackState.Stopped)
                playerActor.Play(libraryItem);

            Pi.Init<BootstrapWiringPi>();
            pin = Pi.Gpio[BcmPin.Gpio03];
            pin.PinMode = GpioPinDriveMode.Input;
            pin.RegisterInterruptCallback(EdgeDetection.FallingEdge, ISRCallback);
            return Task.CompletedTask;
        }

        void ISRCallback()
        {
            _shutdownActor.Initiate(ShutdownType.ShutdownImmediate);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            pin = null;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            //do nothing;
        }
    }
}
