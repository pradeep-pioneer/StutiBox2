using System;
using System.Collections.Generic;
using StutiBox.Api.Actors;

namespace StutiBox.Api.Models
{
    public class PlayerStatusModel
    {
        public bool Status { get; set; }
        public int TotalLibraryItems { get; set; }
        public DateTime LibraryRefreshedAt { get; set; }
        public PlaybackState PlayerState { get; set; }
        public LibraryItem CurrentLibraryItem { get; set; }
        public string BassState { get; set; }
        public byte Volume { get; set; }
        public long CurrentPositionBytes { get; set; }
        public double CurrentPositionSeconds { get; set; }
        public string CurrentPositionString { get; set; }
        public bool Repeat { get; set; }
        public List<LibraryItem> NowPlaying { get; set; }
    }
}