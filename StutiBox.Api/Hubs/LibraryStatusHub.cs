using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using StutiBox.Api.Actors;
using StutiBox.Api.Models;
using System.Collections.Generic;

namespace StutiBox.Api.Hubs
{
    public interface ILibraryStatusHub
    {
        Task ReceiveLibraryStatus(LibraryStatusModel model);
    }
    public class LibraryStatusHub : Hub<ILibraryStatusHub>
    {
        private readonly IPlayerActor PlayerActor;
        public LibraryStatusHub(IPlayerActor playerActor)
        {
            PlayerActor = playerActor;
        }

        private LibraryStatusModel BuildModel(List<LibraryItem> items, bool result=true)
        {
            return new LibraryStatusModel()
            {
                Status = result,
                Items = items,
                LibraryRefreshedAt = PlayerActor.LibraryActor.RefreshedAt
            };
        }

        public async Task<LibraryStatusModel> GetLibraryItems()
        {
            return await Task.FromResult(BuildModel(PlayerActor.LibraryActor.LibraryItems));
        }

        public async Task<LibraryStatusModel> Search(string[] keyWords)
        {
            var items = PlayerActor.LibraryActor.Find(keyWords);
            if(items!=null)
                return await Task.FromResult(BuildModel(items));
            else
                return await Task.FromResult(BuildModel(new List<LibraryItem>(),false));
        }

        public async Task<LibraryStatusModel> QuickSearch(string[] keyWords)
        {
            var items = PlayerActor.LibraryActor.Find(keyWords);
            if(items!=null)
                return await Task.FromResult(BuildModel(items));
            else
                return await Task.FromResult(BuildModel(new List<LibraryItem>(), false));
        }

        public async Task<LibraryStatusModel> GetDetails(int id)
        {
            var item = PlayerActor.LibraryActor.GetItem(id);
            if(item!=null)
                return await Task.FromResult(BuildModel(new List<LibraryItem>(){item}));
            else
                return await Task.FromResult(BuildModel(new List<LibraryItem>(), false));
        }

        public async Task Refresh(bool stopPlayer)
        {
            if(stopPlayer)
			{
				if (PlayerActor.PlaybackState == PlaybackState.Playing || PlayerActor.PlaybackState == PlaybackState.Paused) PlayerActor.Stop();
			}
            bool result = PlayerActor.LibraryActor.Refresh();
            var items = PlayerActor.LibraryActor.LibraryItems;
            var model = BuildModel(items,result);
            await SendLibraryStatus(model);
        }

        public async Task SendLibraryStatus(LibraryStatusModel model)
        {
            await Clients.All.ReceiveLibraryStatus(model);
        }
    }
}