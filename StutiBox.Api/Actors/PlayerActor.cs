using System;
using StutiBox.Api.Models;
using Un4seen.Bass;

namespace StutiBox.Api.Actors
{
	/*
	 * To Do:
	 * 1. Add a wrapper around Bass object to make it testable
	 * 2. Add current status reporting
	 * 3. Add code for playlists
    */
    public class PlayerActor : IPlayerActor
    {
		public ILibraryActor LibraryActor { get; private set;}
		public IBassActor BassActor { get; private set; }
        public PlaybackState PlaybackState { get; internal set; }
		public LibraryItem CurrentLibraryItem { get; private set; }


        public PlayerActor(ILibraryActor libraryActor, IBassActor bassActor)
        {
			BassActor = bassActor;
			BassActor.BassActorEvent += onBassEvent;
			LibraryActor = libraryActor;
            PlaybackState = PlaybackState.NotInitialized;
			if (bassActor.State == ChannelStates.Faulted)
				PlaybackState = PlaybackState.Faulted;
			else
				PlaybackState = PlaybackState.Stopped;
        }

        private void onBassEvent(object sender, BassEventArgs args)
		{
			if (args.Event == BassEvent.PlaybackFinished)
			{
				PlaybackState = PlaybackState.Stopped;
				CurrentLibraryItem = null;
			}
				
			if (args.Event == BassEvent.PlaybackRestarting)
				PlaybackState = PlaybackState.Playing;
		}

        public bool Play(int identifier)
        {
			var item = LibraryActor.GetItem(identifier);
			var result = BassActor.Play(item.FullPath);
			if (result)
			{
				CurrentLibraryItem = item;
				PlaybackState = PlaybackState.Playing;
			}
			return result;
        }

		public bool Pause()
		{
			var result = BassActor.Pause();
			if (result)
				PlaybackState = PlaybackState.Paused;
			return result;
		}

        public bool Resume()
		{
			var result = BassActor.Resume();
			if (result)
				PlaybackState = PlaybackState.Playing;
			return result;
		}

        public bool Stop()
        {
			var result = BassActor.Stop();
			if (result)
			{
				CurrentLibraryItem = null;
				PlaybackState = PlaybackState.Stopped;
			}
			return result;
        }

		public bool Volume(byte volume) => BassActor.Volume(volume);

		public bool ToggleRepeat() => BassActor.ToggleRepeat();

		public bool Seek(double seconds) => BassActor.Seek(seconds);

		public bool ConversationStarted()
		{
			if (PlaybackState == PlaybackState.Playing)
				return Pause();
			return true;
		}

		public bool ConversationFinished()
		{
			if (PlaybackState == PlaybackState.Paused)
				return Resume();
			return true;
		}
    }

    public enum PlaybackState
    {
        NotInitialized = -1,
        Faulted = 0,
        Stopped = 1,
        Playing = 2,
        Paused = 3
    }
}
