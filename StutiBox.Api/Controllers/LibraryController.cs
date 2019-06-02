using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using StutiBox.Api.Actors;
using StutiBox.Api.Models;
using Microsoft.AspNetCore.Cors;

namespace StutiBox.Api.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    public class LibraryController : Controller
    {
        public LibraryController(IPlayerActor player)
        {
            PlayerActor = player;
        }
		public IPlayerActor PlayerActor{ get; private set; }

		[HttpGet]
        [Route("")]
        [Route("List")]
        public IActionResult List()
        {
			return Ok(new { Status = true, LibraryRefreshedAt = PlayerActor.LibraryActor.RefreshedAt, Items = PlayerActor.LibraryActor.LibraryItems });
        }

        [HttpPost]
        [Route("Search")]
        public IActionResult Search([FromBody]string[] keyWords)
        {
			var result = new { Status = true, LibraryRefreshedAt = PlayerActor.LibraryActor.RefreshedAt, Items = PlayerActor.LibraryActor.Find(keyWords) };
            if (result.Items.Count > 0)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPost]
        [Route("QuickSearch")]
        public IActionResult QuickSearch([FromBody]string[] keyWords)
        {
			var result = new { Status = true, LibraryRefreshedAt = PlayerActor.LibraryActor.RefreshedAt, Items = PlayerActor.LibraryActor.LuckySearch(keyWords) };
            if (result != null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
			var result = new { Status = true, LibraryRefreshedAt = PlayerActor.LibraryActor.RefreshedAt, Item = PlayerActor.LibraryActor.GetItem(id) };
            if (result.Item != null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("Refresh")]
        public IActionResult Refresh(bool stopPlayer=false)
		{
			if(stopPlayer)
			{
				if (PlayerActor.PlaybackState == PlaybackState.Playing || PlayerActor.PlaybackState == PlaybackState.Paused) PlayerActor.Stop();
			}
			bool result = PlayerActor.LibraryActor.Refresh();
			var message = result ? $"Library Refreshed!" : "Operation Failed - check status field for details!";
			var status = new { PlayerState = PlayerActor.PlaybackState, LibraryItemsCount = PlayerActor.LibraryActor.LibraryItems.Count };
			var response = new { Status = result, Message = message, Items = PlayerActor.LibraryActor.LibraryItems };
			return Ok(response);
		}

    }
}
