using System;
using StutiBox.Api.Models;

namespace StutiBox.Api.Actors
{
	public interface IBassActor
	{
		ChannelStates State { get; set; }
		byte CurrentVolume { get; }
		string CurrentPositionString { get; }
		double CurrentPositionSeconds { get; }
		long CurrentPositionBytes { get; }
		bool Repeat { get; }
		EventHandler<BassEventArgs> BassActorEvent { get; set; }
		string[] GetTags(int stream,bool closeStream = false);
		string[] GetTags(string fullPath);
		int GetStream(string fullPath);
		dynamic GetFullTrackInformation(string fullPath);
		bool ToggleRepeat();
		bool Play(string fullPath);
		bool Pause();
		bool Resume();
		bool Stop();


		bool Volume(byte volume);
		bool Seek(double seconds);
	}

	public enum ChannelStates
	{
		NotInitialized=-0,
        Ready,
        Playing,
        Paused,
        Faulted
	}
}
