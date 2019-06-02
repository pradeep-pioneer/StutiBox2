using System;
using Un4seen.Bass;
using System.IO;
using System.Text;
using StutiBox.Api.Models;

namespace StutiBox.Api.Actors
{
	public class BassActor:IBassActor
    {
		public const byte MAX_VOLUME = 100;

		public EventHandler<BassEventArgs> BassActorEvent { get; set; }
		public ChannelStates State { get; set; }
		public bool Repeat { get; private set; }
		public int Stream { get; private set; }
		public long CurrentPositionBytes => Stream != 0 ? Bass.BASS_ChannelGetPosition(Stream) : 0;
		public byte CurrentVolume => (byte)(Bass.BASS_GetVolume()*100f);
		public string CurrentPositionString
		{
			get
			{
				if (State == ChannelStates.Playing || State == ChannelStates.Paused)
				{
					var timeSpan = TimeSpan.FromSeconds(CurrentPositionSeconds);
					return timeSpan.ToString(timeSpan.Hours > 0 ? @"hh\:mm\:ss" : @"mm\:ss");
				}
                else if (State == ChannelStates.Ready)
					return "--:--:--";
                else
					return "(..):(..)";
			}
		}
		public double CurrentPositionSeconds
		{
			get
			{
				if (State == ChannelStates.Playing || State == ChannelStates.Paused)
					return Bass.BASS_ChannelBytes2Seconds(Stream, CurrentPositionBytes);
				else if (State == ChannelStates.Ready)
					return 0d;
				else
					return -1d;
			}
		}

		public BassActor()
		{
			State = ChannelStates.NotInitialized;
			setupPlayBackEngine();
		}

		private void setupPlayBackEngine()
        {
            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                State = ChannelStates.Faulted;
                throw new NotSupportedException("Initialization of Playback Engine Failed");
            }

            State = ChannelStates.Ready;
			Repeat = true;
        }

        public string[] GetTags(int stream, bool closeStream = false)
		{
			if (State == ChannelStates.Faulted || State == ChannelStates.NotInitialized)
				throw new InvalidOperationException($"State: {State.ToString()}");
			if (stream != 0)
			{
				var tags = Bass.BASS_ChannelGetTagsID3V1(stream);
				if (closeStream)
					Bass.BASS_StreamFree(stream);
				if (tags != null)
					return tags;
				else
					return new string[]{};
			}
			else
				throw new InvalidDataException("Stream not valid!");
		}

        public string[] GetTags(string fullPath)
		{
			if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not not found!", fullPath);
			var stream = Bass.BASS_StreamCreateFile(fullPath, 0, 0, BASSFlag.BASS_DEFAULT);
			return GetTags(stream,true);
		}

        public int GetStream(string fullPath)
		{
			if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not not found!", fullPath);
			if (State == ChannelStates.Faulted || State == ChannelStates.NotInitialized)
                throw new InvalidOperationException($"State: {State.ToString()}");
			return Bass.BASS_StreamCreateFile(fullPath, 0, 0, BASSFlag.BASS_DEFAULT);
		}

		public dynamic GetFullTrackInformation(string fullPath)
		{
			var stream = GetStream(fullPath);
			var tags = GetTags(stream, false) ?? new string[] { };
			var lengthBytes = Bass.BASS_ChannelGetLength(stream);
			var lengthSeconds = Bass.BASS_ChannelBytes2Seconds(stream, lengthBytes);
			var timeSpan = TimeSpan.FromSeconds(lengthSeconds);
			var lengthTimeString = timeSpan.ToString(timeSpan.Hours > 0 ? @"hh\:mm\:ss" : @"mm\:ss");
			return new { Tags = tags, LengthBytes = lengthBytes, LengthSeconds = lengthSeconds, LengthTimeString = lengthTimeString };
		}

		public bool ToggleRepeat()
		{
			if (State != ChannelStates.Faulted && State != ChannelStates.NotInitialized)
			{
				Repeat = !Repeat;
				return true;
			}
			else
				return false;
		}

        public bool Play(string fullPath)
		{
			if (State != ChannelStates.Ready)
                throw new NotSupportedException($"ChannelState not valid! [ChannelState = {State.ToString()}]");
			Stream = GetStream(fullPath);
			if (Stream != 0)
			{
				var playBackStartSyncProc = new SYNCPROC(playBackStartCallback);
				var playBackEndSyncProc = new SYNCPROC(playBackEndCallBack);
				var playBackPausedSyncProc = new SYNCPROC(playBackPausedCallBack);
				Bass.BASS_ChannelSetSync(Stream, BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME, 0, playBackStartSyncProc, IntPtr.Zero);
                Bass.BASS_ChannelSetSync(Stream, BASSSync.BASS_SYNC_END | BASSSync.BASS_SYNC_MIXTIME, 0, playBackEndSyncProc, IntPtr.Zero);
				Bass.BASS_ChannelSetSync(Stream, BASSSync.BASS_SYNC_STALL | BASSSync.BASS_SYNC_MIXTIME, 0, playBackPausedSyncProc, IntPtr.Zero);
				if (Bass.BASS_ChannelPlay(Stream, false))
					State = ChannelStates.Playing;
				return true;
			}
			else
				return false;
		}

		public bool Pause()
        {
            if (State != ChannelStates.Playing)
                throw new NotSupportedException($"Cannot pause playback! [Current State: {State.ToString()}]");
            var result = Stream != 0 ? Bass.BASS_ChannelPause(Stream) : false;
            if (result)
				State = ChannelStates.Paused;
            return result;
        }

        public bool Resume()
		{
			if (State != ChannelStates.Paused)
                throw new NotSupportedException($"Cannot resume playback! [Current State: {State.ToString()}]");
            var result = Stream != 0 ? Bass.BASS_ChannelPlay(Stream, false) : false;
            if (result)
				State = ChannelStates.Playing;
            return result;
		}

		public bool Stop()
        {
            bool result = false;
            if (State == ChannelStates.Playing || State == ChannelStates.Paused)
            {
                if (Stream != 0)
                {
                    result = Bass.BASS_ChannelStop(Stream);
                    result = Bass.BASS_StreamFree(Stream);
                    State = ChannelStates.Ready;
                }
                else
                    result = false;
            }
            else
                throw new NotSupportedException($"Cannot stop playback! [Current State: {State.ToString()}]");
            Stream = 0;
            return result;
        }

		public bool Volume(byte volume)
        {
			float actualVolume;
            if (volume < 0)
				actualVolume = 0;
            else if (volume > MAX_VOLUME)
				actualVolume = MAX_VOLUME;
            else
				actualVolume = volume;
            actualVolume = (float)actualVolume / 100f;
            var result = Bass.BASS_SetVolume(actualVolume);
            return result;
        }

		private void playBackPausedCallBack(int handle, int channel, int data, IntPtr user)
        {
            if (data == 0) { State = ChannelStates.Paused; }
            else { State = ChannelStates.Playing; }
        }

        private void playBackStartCallback(int handle, int channel, int data, IntPtr user)
        {
            State = ChannelStates.Playing;

        }

        private void playBackEndCallBack(int handle, int channel, int data, IntPtr user)
        {
			try
			{
				var eventType = BassEvent.PlaybackFinished;
				if (Repeat)
				{
					Seek(0);
					Bass.BASS_ChannelPlay(Stream, true);
					eventType = BassEvent.PlaybackRestarting;
					State = ChannelStates.Playing;
				}
				else
					cleanupStream();
				if (this.BassActorEvent != null)
					BassActorEvent(this, new BassEventArgs(eventType));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unhandled: {ex.Message}\nStackTrace: {ex.StackTrace}");
			}
        }

        public bool Seek(double seconds)
		{
			if (State == ChannelStates.Playing || State == ChannelStates.Paused)
			{
				var bytes = Bass.BASS_ChannelSeconds2Bytes(Stream, seconds);
				return Bass.BASS_ChannelSetPosition(Stream, bytes, BASSMode.BASS_POS_BYTE);
			}
			else
				return false;
		}

        private void cleanupStream()
		{
			cleanupStream(Stream);
			Stream = 0;
			State = ChannelStates.Ready;

		}

		private void cleanupStream(int stream)
		{
			if (stream != 0)
				Bass.BASS_ChannelStop(stream);
		}
    }
}
