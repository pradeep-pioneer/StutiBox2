using System;
namespace StutiBox.Api.Config
{
    //this needs to be persisted - for now using hard coded values
    public class RestartConfiguration
    {
        public TimeSpan RestartTime { get; set; }
        public bool Enabled { get; set; }
        public TimeSpan CheckFrequency { get; set; }
    }
}
