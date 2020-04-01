using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StutiBox.Api.Actors;
using StutiBox.Api.Config;

namespace StutiBox.Api.Workers
{
    public class MorningAlarmWorker : IHostedService, IDisposable
    {
        private readonly IPlayerActor playerActor;
        private readonly ILibraryActor libraryActor;
        private readonly ILogger<MorningAlarmWorker> logger;
        private Timer _timer;
        private bool alarmTriggered = false;
        public AlarmConfiguration AlarmConfiguration { get; set; }
        private byte previousVolume = 0;
        public MorningAlarmWorker(IPlayerActor playerActor, ILogger<MorningAlarmWorker> logger, ILibraryActor libraryActor)
        {
            this.playerActor = playerActor;
            this.logger = logger;
            this.libraryActor = libraryActor;
            this.previousVolume = playerActor.BassActor.CurrentVolume;
            this.AlarmConfiguration = new AlarmConfiguration()
            {
                AlarmTime = TimeSpan.FromHours(6),
                AlarmMissThreshold = TimeSpan.FromMinutes(20),
                AlarmAutoTurnOffCheckTime = TimeSpan.FromHours(8),
                Enabled = true,
                MediaItemId = 1
            };
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
            var shouldTrigger = time.Hour == AlarmConfiguration.AlarmTime.Hours
                && time.Minute >= AlarmConfiguration.AlarmTime.Minutes
                && time.Minute <= time.Minute + AlarmConfiguration.AlarmMissThreshold.Minutes
                && !alarmTriggered;
            if (shouldTrigger)
            {
                logger.LogInformation("Raising alarm");
                alarmTriggered = true;
                if (playerActor.PlaybackState == PlaybackState.Playing || playerActor.PlaybackState == PlaybackState.Paused)
                    playerActor.Stop();
                if (!playerActor.Repeat)
                    playerActor.ToggleRepeat();
                var libraryItem = libraryActor.GetItem(AlarmConfiguration.MediaItemId);
                this.previousVolume = playerActor.BassActor.CurrentVolume;
                playerActor.Volume(100);
                playerActor.Play(libraryItem);
            }
            else if (time.Hour == AlarmConfiguration.AlarmAutoTurnOffCheckTime.Hours && time.Minute >= AlarmConfiguration.AlarmAutoTurnOffCheckTime.Minutes && alarmTriggered)
            {
                logger.LogInformation("Disarming alarm");
                playerActor.Volume(previousVolume);
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
