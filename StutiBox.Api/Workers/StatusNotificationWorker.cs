using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using StutiBox.Api.Hubs;
using StutiBox.Api.Actors;
using System.Threading.Tasks;
using System.Threading;
using StutiBox.Api.Models;

namespace StutiBox.Api.Workers
{
    public class StatusNotificationWorker : BackgroundService
    {
        private readonly IHubContext<PlayerStatusHub, IPlayerStatusHub> PlayerStatusHub;
        private readonly IPlayerActor PlayerActor;
        public StatusNotificationWorker(IHubContext<PlayerStatusHub, IPlayerStatusHub> playerHub, IPlayerActor playerActor)
        {
            PlayerStatusHub = playerHub;
            PlayerActor = playerActor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await PlayerStatusHub.Clients.All.ReceivePlaybackStatus(getPlayerStatus());
                await Task.Delay(500);
            }
        }
        private PlayerStatusModel getPlayerStatus()
        {
            return new PlayerStatusModel
            {
                Status = true,
                TotalLibraryItems = PlayerActor.LibraryActor.LibraryItems.Count,
                LibraryRefreshedAt = PlayerActor.LibraryActor.RefreshedAt,
                PlayerState = PlayerActor.PlaybackState,
                CurrentLibraryItem = PlayerActor.CurrentLibraryItem,
                BassState = PlayerActor.BassActor.State.ToString(),
                Volume = PlayerActor.BassActor.CurrentVolume,
                CurrentPositionBytes = PlayerActor.BassActor.CurrentPositionBytes,
                CurrentPositionSeconds = PlayerActor.BassActor.CurrentPositionSeconds,
                CurrentPositionString = PlayerActor.BassActor.CurrentPositionString,
                Repeat = PlayerActor.Repeat
            };
        }
    }
}