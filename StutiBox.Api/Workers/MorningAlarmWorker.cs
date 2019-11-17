using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StutiBox.Api.Actors;

namespace StutiBox.Api.Workers
{
    public class MorningAlarmWorker : IHostedService, IDisposable
    {
        private readonly IPlayerActor playerActor;
        private readonly ILibraryActor libraryActor;
        private readonly ILogger<MorningAlarmWorker> logger;
        private Timer _timer;
        private bool alarmTriggered = false;
        public MorningAlarmWorker(IPlayerActor playerActor, ILogger<MorningAlarmWorker> logger, ILibraryActor libraryActor)
        {
            this.playerActor = playerActor;
            this.logger = logger;
            this.libraryActor = libraryActor;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.Log(LogLevel.Information, "Started morning alarm!");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {

            var time = DateTime.Now;
            if (time.Hour == 1 && time.Minute > 0 && !alarmTriggered)
            {
                logger.LogInformation("Raising alarm");
                alarmTriggered = true;
                if (playerActor.PlaybackState == PlaybackState.Playing || playerActor.PlaybackState == PlaybackState.Paused)
                    playerActor.Stop();
                var libraryItem = libraryActor.GetItem(1);
                playerActor.Play(libraryItem);
            }
            else if (time.Hour == 8 && time.Minute == 0 && alarmTriggered)
            {
                logger.LogInformation("Disarming alarm");
                alarmTriggered = false;
                if (playerActor.PlaybackState == PlaybackState.Playing || playerActor.PlaybackState == PlaybackState.Paused)
                    playerActor.Stop();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping alarm service.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
