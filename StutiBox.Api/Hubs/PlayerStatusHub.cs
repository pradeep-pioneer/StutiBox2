using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using StutiBox.Api.Actors;
using StutiBox.Api.Models;

namespace StutiBox.Api.Hubs
{
    public interface IPlayerStatusHub
    {
        Task ReceivePlaybackStatus(PlayerStatusModel model);
    }
    public class PlayerStatusHub : Hub<IPlayerStatusHub>
    {
        IPlayerActor PlayerActor;
		ILibraryActor LibraryActor;
        public PlayerStatusHub(IPlayerActor playerActor, ILibraryActor libraryActor)
        {
            PlayerActor = playerActor;
			LibraryActor = libraryActor;
        }

        public async Task<PlayerStatusModel> RequestAction(PlayerRequest playerRequest)
        {
            switch (playerRequest.RequestType)
            {
                case RequestType.Play:
					if (PlayerActor.PlaybackState == PlaybackState.Stopped)
					    PlayerActor.Play(playerRequest.Identifier);
                    break;
                case RequestType.Pause:
					if(PlayerActor.PlaybackState == PlaybackState.Playing)
					    PlayerActor.Pause();
                    break;
				case RequestType.Resume:
					if (PlayerActor.PlaybackState == PlaybackState.Paused)
                        PlayerActor.Resume();
                    break;
                case RequestType.Stop:
					if (PlayerActor.PlaybackState == PlaybackState.Playing || PlayerActor.PlaybackState == PlaybackState.Paused)
					    PlayerActor.Stop();
                    break;
                case RequestType.Enqueue:
                    PlayerActor.Enqueue(playerRequest.Identifier);
                    break;
                case RequestType.DeQueue:
                    break;
                default:
                    break;
            }
            await SendPlaybackStatus();
            return await Task.FromResult(getPlayerStatus());
        }

        public async Task<PlayerStatusModel> ControlAction(PlayerControlRequest playerControlRequest)
        {
            switch (playerControlRequest.ControlRequest)
			{
				case ControlRequest.VolumeAbsolute:
					var volume = (byte)playerControlRequest.RequestData;
					PlayerActor.Volume(volume);
					break;
				case ControlRequest.RepeatToggle:
					PlayerActor.ToggleRepeat();
                    break;
				case ControlRequest.Seek:
					PlayerActor.Seek(playerControlRequest.RequestData);
					break;
				case ControlRequest.VolumeRelative:
					var volumeStep = (byte)playerControlRequest.RequestData;
					var oldVolume = PlayerActor.BassActor.CurrentVolume;
					var newVolume = (byte)(oldVolume + volumeStep);
					PlayerActor.Volume(newVolume);
                    break;
				case ControlRequest.Random:
				default:
					break;
			}
            await SendPlaybackStatus();
            return await Task.FromResult(getPlayerStatus());
        }

        public async Task<PlayerStatusModel> RequestPlaybackStatus()
        {
            return await Task.FromResult(getPlayerStatus());
        }

        public async Task SendPlaybackStatus()
        {
            await Clients.All.ReceivePlaybackStatus(getPlayerStatus());
        }
        private PlayerStatusModel getPlayerStatus()
        {
            return new PlayerStatusModel
			{
                Status = true,
                TotalLibraryItems = LibraryActor.LibraryItems.Count,
                LibraryRefreshedAt = LibraryActor.RefreshedAt,
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