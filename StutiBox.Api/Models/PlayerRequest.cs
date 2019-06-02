using System;
using System.ComponentModel.DataAnnotations;

namespace StutiBox.Api.Models
{
    public class PlayerRequest
    {
		[Required]
        public RequestType RequestType { get; set; }
        public int Identifier { get; set; }
    }

	public enum RequestType
	{
		Play = 0,
		Pause = 1,
		Stop = 2,
		Resume = 3,
		Enqueue = 4,
		DeQueue = 5
    }
}
