using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StutiBox.Api.Models;
using Microsoft.Extensions.Options;
using StutiBox.Api.Config;

namespace StutiBox.Api.Actors
{
    public class LibraryActor : ILibraryActor
    {
        private LibraryConfiguration Config;
        private IBassActor bassActor;
        public DateTime RefreshedAt { get; private set; }
        public LibraryActor(IOptionsMonitor<LibraryConfiguration> config, IBassActor bass)
        {

            Config = config.CurrentValue;
            bassActor = bass ?? throw new ArgumentNullException(nameof(bass));
            buildLibrary(bass);
        }

        public LibraryItem this[int id] => GetItem(id);

        public LibraryItem GetItem(int id) => LibraryItems.FirstOrDefault(x => x.Id == id);

        public List<LibraryItem> LibraryItems { get; private set; }

        public List<LibraryItem> Find(params string[] keywords)
        {
            List<LibraryItem> results = new List<LibraryItem>();
            keywords.ToList().ForEach(item =>
            {
                var items = LibraryItems.Where(x => x.Name.ToLower().Contains(item.ToLower()));
                results.AddRange(items);
            });
            return results.Distinct().ToList();
        }

        public LibraryItem LuckySearch(params string[] keywords)
        {
            return LibraryItems.FirstOrDefault(item => keywords.Any(that => item.Name.ToLower().Contains(that.ToLower())));
        }

        public bool Refresh()
        {
            bool result;

            try
            {
                buildLibrary(bassActor);
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private void buildLibrary(IBassActor bass)
        {
            LibraryItems = new List<LibraryItem>();
            int counter = 1;
            var directory = new DirectoryInfo(Config.MusicDirectory);
            var comparison = new Comparison<FileInfo>((x, y) => { return x.Name.CompareTo(y.Name); });
            var musicFiles = directory.EnumerateFiles("*.mp3", SearchOption.AllDirectories).ToList();
            musicFiles.Sort(comparison);
            var items = musicFiles.OrderBy(x => x.Name).Select(x => new LibraryItem(counter++, x.FullName, bassActor)).ToList();
            LibraryItems.AddRange(items);
            RefreshedAt = DateTime.Now;
        }
    }
}
