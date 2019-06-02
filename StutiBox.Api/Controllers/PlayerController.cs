using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StutiBox.Api.Actors;
using StutiBox.Api.Models;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//Top features required:
//1. Alarms: Basically scheduled timers for music playback
//2. reminders creation and playback: using could speech/any free

namespace StutiBox.Api.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    public class PlayerController : Controller
    {
        IPlayerActor PlayerActor;
		ILibraryActor LibraryActor;

        public PlayerController(IPlayerActor playerActor, ILibraryActor libraryActor)
        {
            PlayerActor = playerActor;
			LibraryActor = libraryActor;
        }

        [HttpGet]
        [Route("")]
        [Route("Status")]
        public IActionResult Status()
        {
			var response = new
			{
                Status = true,
                TotalLibraryItems = LibraryActor.LibraryItems.Count,
                LibraryRefreshedAt = LibraryActor.RefreshedAt,
                PlayerState = PlayerActor.PlaybackState,
                PlayerActor.CurrentLibraryItem,
				BassState = PlayerActor.BassActor.State.ToString(),
                Volume = PlayerActor.BassActor.CurrentVolume,
                PlayerActor.BassActor.CurrentPositionBytes,
                PlayerActor.BassActor.CurrentPositionSeconds,
                PlayerActor.BassActor.CurrentPositionString,
                PlayerActor.BassActor.Repeat
			};
            return Ok(response);
        }

        [HttpGet]
        [Route("ConversationStarted")]
        public IActionResult ConversationStarted()
		{
			var respose = new
			{
				Status = PlayerActor.ConversationStarted(),
				Volume = PlayerActor.BassActor.CurrentVolume
			};
			return Ok(respose);
		}

		[HttpGet]
        [Route("ConversationFinished")]
        public IActionResult ConversationFinished()
        {
            var respose = new
            {
                Status = PlayerActor.ConversationFinished(),
                Volume = PlayerActor.BassActor.CurrentVolume
            };
            return Ok(respose);
        }

        [HttpPost]
        [Route("Request")]
        public IActionResult RequestAction([FromBody]PlayerRequest playerRequest)
        {
			dynamic response = null;
            switch (playerRequest.RequestType)
            {
                case RequestType.Play:
					if (PlayerActor.PlaybackState == PlaybackState.Stopped)
					{
						var success = PlayerActor.Play(playerRequest.Identifier);
						if (success)
							response = new { Status = success, Message = $"Started!", MediaItem = PlayerActor.CurrentLibraryItem };
						else
							response = new { Status = success, Message = "Unknown Error" };
					}
					else
						response = new { Status = false, Message = $"Invalid State!", State = PlayerActor.PlaybackState.ToString() };
                    break;
                case RequestType.Pause:
					if(PlayerActor.PlaybackState == PlaybackState.Playing)
					{
						var success = PlayerActor.Pause();
                        if (success)
							response = new { Status = success, Message = $"Playback Paused!", MediaItem = PlayerActor.CurrentLibraryItem };
                        else
                            response = new { Status = success, Message = "Unknown Error" };
					}
					else
                        response = new { Status = false, Message = $"Invalid State!", State = PlayerActor.PlaybackState.ToString() };
                    break;
				case RequestType.Resume:
					if (PlayerActor.PlaybackState == PlaybackState.Paused)
                    {
                        var success = PlayerActor.Resume();
                        if (success)
							response = new { Status = success, Message = $"Playback Resuming!", MediaItem = PlayerActor.CurrentLibraryItem };
                        else
                            response = new { Status = success, Message = "Unknown Error" };
                    }
					else
                        response = new { Status = false, Message = $"Invalid State!", State = PlayerActor.PlaybackState.ToString() };
                    break;
                case RequestType.Stop:
					if (PlayerActor.PlaybackState == PlaybackState.Playing || PlayerActor.PlaybackState == PlaybackState.Paused)
					{
						var currentItem = PlayerActor.CurrentLibraryItem;
						var success = PlayerActor.Stop();
						if (success)
						{
							//Todo: Add the capability to say which song was stopped
							response = new { Status = success, Message = "Playback stopped!", MediaItem = currentItem };
						}
						else
							response = new { Status = success, Message = "Unknown Error" }; //Todo: add ability to provide error message as well
					}
					else
						response = new { Status = false, Message = $"Invalid State", State = PlayerActor.PlaybackState.ToString() };
                    break;
                case RequestType.Enqueue:
                    break;
                case RequestType.DeQueue:
                    break;
                default:
					return BadRequest(new { Status = false, Message = "Unknown RequestType" });
            }
            return Ok(response);
        }

		[HttpPost]
		[Route("Control")]
		public IActionResult PlayerControlAction([FromBody]PlayerControlRequest playerControlRequest)
		{
			dynamic response = null;
            switch (playerControlRequest.ControlRequest)
			{
				case ControlRequest.VolumeAbsolute:
					var volume = (byte)playerControlRequest.RequestData;
					if (PlayerActor.Volume(volume))
						response = new { Status = true, Message = $"Set Volume!", Values = new {Scale1 = PlayerActor.BassActor.CurrentVolume, Scale2 = (float)PlayerActor.BassActor.CurrentVolume / 100f} };
					else
						response = new { Status = false, Message = $"Unknown Error!" };
					break;
				case ControlRequest.RepeatToggle:
					var result = PlayerActor.ToggleRepeat();
                    response = new
                    {
                        Status = result,
                        Message = result ? $"Success! Repeat: {PlayerActor.BassActor.Repeat}" : $"Failed! {PlayerActor.PlaybackState.ToString()}",
						PlayerActor.BassActor.Repeat
                    };
                    break;
				case ControlRequest.Seek:
					result = PlayerActor.Seek(playerControlRequest.RequestData);
					response = new
                    {
                        Status = result,
						Message = result ? $"Success! Seek: {playerControlRequest.RequestData}" : $"Failed to seek to: {playerControlRequest.RequestData.ToString()}!",
						PlayerActor.BassActor.CurrentPositionBytes,
                        PlayerActor.BassActor.CurrentPositionSeconds,
                        PlayerActor.BassActor.CurrentPositionString
                    };
					break;
				case ControlRequest.VolumeRelative:
					var volumeStep = (byte)playerControlRequest.RequestData;
					var oldVolume = PlayerActor.BassActor.CurrentVolume;
					var newVolume = (byte)(oldVolume + volumeStep);
					if (PlayerActor.Volume(newVolume))
						response = new { Status = true, Message = $"Set Volume!", Values = new { Scale1 = PlayerActor.BassActor.CurrentVolume, Scale2 = (float)PlayerActor.BassActor.CurrentVolume / 100f } };
                    else
                        response = new { Status = false, Message = $"Unknown Error!" };
                    break;
				case ControlRequest.Random:
				default:
					break;
			}
			return Ok(response);
		}
    }
}
