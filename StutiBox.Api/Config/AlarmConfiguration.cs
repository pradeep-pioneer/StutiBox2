using System;
namespace StutiBox.Api.Config
{
    //this needs to be persisted - for now using hard coded values
    public class AlarmConfiguration
    {
        public TimeSpan AlarmTime { get; set; }
        public TimeSpan AlarmMissThreshold { get; set; }
        public TimeSpan AlarmAutoTurnOffCheckTime { get; set; }
        public bool Enabled { get; set; }
        public int MediaItemId { get; set; }
    }
}
