using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;
using StutiBox.Api.Models;

namespace StutiBox.Api.Actors
{
    public class ShutdownActor : IShutdownActor
    {
        private const string ShutdownFileName = "shutdown.mp3";
        private readonly IPlayerActor _playerActor;
        private readonly IBassActor _bassActor;
        private readonly ILogger<ShutdownActor> _logger;
        private bool _isShutdownRequested = false;
        private readonly string _appDirectory;

        public ShutdownActor(IPlayerActor playerActor, ILogger<ShutdownActor> logger, IBassActor bassActor)
        {
            _playerActor = playerActor;
            _logger = logger;
            _bassActor = bassActor;
            _appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void Initiate(ShutdownType shutdownType)
        {
            //debounce
            if (!_isShutdownRequested)
            {
                _isShutdownRequested = true;
                _logger.Log(LogLevel.Information, $"{shutdownType} signal received");
                var libraryItem = new LibraryItem(-1, Path.Combine(_appDirectory, ShutdownFileName), _bassActor);
                if (_playerActor.PlaybackState == PlaybackState.Playing || _playerActor.PlaybackState == PlaybackState.Paused)
                    _playerActor.Stop();
                if (_playerActor.PlaybackState == PlaybackState.Stopped)
                    _playerActor.Play(libraryItem);
                Thread.Sleep(2000);
                var operation = shutdownType == ShutdownType.RestartImmediate ? "reboot" : "shutdown";
                Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = $"{operation} now" });
            }
        }
    }
}
