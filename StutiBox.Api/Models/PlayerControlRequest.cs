using System;
namespace StutiBox.Api.Models
{
	public class PlayerControlRequest
	{

		public ControlRequest ControlRequest { get; set; }
		public double RequestData { get; set; }
	}

	public enum ControlRequest
	{
		VolumeAbsolute = 1,
		RepeatToggle = 2,
		Random = 3,
        Seek = 4,
        VolumeRelative = 5,
        RepeatAbsolute = 6
	}
}
