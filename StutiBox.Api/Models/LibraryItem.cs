using System;
using System.Collections.Generic;
using System.IO;
using StutiBox.Api.Actors;
using System.Linq;

namespace StutiBox.Api.Models
{
    public class LibraryItem
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
		public string FullPath { get; private set; }
		public long LengthBytes { get; private set; }
		public double LengthSeconds { get; private set; }
		public string LengthTimeString { get; private set; }
		public List<(string, string)> Tags { get; private set; }
		//public string[] keyWords { get; private set; }

		#region [ Construction ]

        public LibraryItem(int id, string fullPath, IBassActor bass)
		{
			if (!File.Exists(fullPath))
				throw new FileNotFoundException("File not found!", fullPath);
			FullPath = fullPath;
			Id = id;
			dynamic information = null;
			try
			{
				information = bass.GetFullTrackInformation(fullPath);
				if (information.Tags is List<string> tags)
				{
					Tags = new List<(string, string)>();
					Name = tags[0] ?? (new FileInfo(fullPath)).Name;
					if (string.IsNullOrEmpty(Name))
						Name = (new FileInfo(fullPath)).Name;
					int acc = 1;
					tags.ToList().ForEach(x =>
					{
						processTag(x, acc);
					});
				}
				else
					Name = (new FileInfo(fullPath)).Name;
				LengthBytes = information.LengthBytes;
				LengthSeconds = information.LengthSeconds;
				LengthTimeString = information.LengthTimeString;

			}
			catch(NotSupportedException ex)
			{
				if (!Console.IsErrorRedirected)
					Console.Error.WriteLine($"LibraryItem({id},{fullPath}, [bass]) => Error ({ex.GetType()}:{ex.Message})");
				if (!Console.IsOutputRedirected)
					Console.WriteLine($"Tags are not available for {fullPath}\nUsing the file name for the information");
				Name = (new FileInfo(fullPath)).Name;
                if(information!=null)
				{
					LengthBytes = information.LengthBytes;
                    LengthSeconds = information.LengthSeconds;
                    LengthTimeString = information.LengthTimeString;
				}
			}
			//keyWords = Name?.Replace(".mp3", "", StringComparison.InvariantCultureIgnoreCase).Split(' ', '_', '-');
		}

        private void processTag(string tag, int index)
		{
            switch (index++)
            {
                case 1:
                    Tags.Add(("Artist", tag ?? string.Empty));
                    break;
                case 2:
                    Tags.Add(("Album", tag ?? string.Empty));
                    break;
                case 3:
                    Tags.Add(("Year", tag ?? string.Empty));
                    break;
                case 4:
                    Tags.Add(("Comments", tag ?? string.Empty));
                    break;
                case 5:
                    Tags.Add(("Genre", tag ?? string.Empty));
                    break;
                case 6:
                    Tags.Add(("Track", tag ?? string.Empty));
                    break;
                default:
                    break;
            }
        }

        #endregion

		public override bool Equals(object obj)
		{
            return (obj as LibraryItem).FullPath == this.FullPath;
		}

		public override int GetHashCode()
		{
			return FullPath.GetHashCode();
		}
	}
}
