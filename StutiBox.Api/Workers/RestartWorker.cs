using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StutiBox.Api.Actors;
using StutiBox.Api.Config;

namespace StutiBox.Api.Workers
{
    public class RestartWorker : IHostedService, IDisposable
    {
        private readonly ILogger<RestartWorker> _logger;
        private Timer _timer;
        public TimeSpan _upTime { get; set; }

        public RestartConfiguration RestartConfiguration { get; protected set; }
        private IShutdownActor _shutdownActor { get; set; }

        public RestartWorker(ILogger<RestartWorker> logger, IShutdownActor shutdownActor)
        {
            _logger = logger;
            _upTime = TimeSpan.FromMinutes(0);
            _shutdownActor = shutdownActor;
            RestartConfiguration = new RestartConfiguration()
            {
                RestartTime = TimeSpan.FromHours(12),
                Enabled = true,
                CheckFrequency = TimeSpan.FromMinutes(30)
            };
        }

        private void DoWork(object state)
        {
            if (_upTime >= RestartConfiguration.RestartTime && RestartConfiguration.Enabled)
                _shutdownActor.Initiate(ShutdownType.RestartImmediate);
            _upTime = _upTime.Add(RestartConfiguration.CheckFrequency);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, "Started restart manager service!");
            if (RestartConfiguration.Enabled)
                _timer = new Timer(DoWork, null, TimeSpan.Zero, RestartConfiguration.CheckFrequency);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping restart manager service.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
