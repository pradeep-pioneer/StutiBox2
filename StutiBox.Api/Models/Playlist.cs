using System;
using StutiBox.Api.Actors;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
namespace StutiBox.Api.Models
{
    public class Playlist
    {
        public int CurrentIndex { get; private set; }
        public ConcurrentDictionary<int, LibraryItem> Items { get; private set; }
        public int Count { get => Items.Count; }
        public Playlist()
        {
            Items = new ConcurrentDictionary<int, LibraryItem>();
            CurrentIndex = -1;
        }

        public KeyValuePair<int, LibraryItem> Enqueue(LibraryItem item)
        {
            if (CurrentIndex < 0)
                CurrentIndex++;
            var index = Items.Count;
            Items[index] = item;
            return new KeyValuePair<int, LibraryItem>(index, item);
        }

        public void Dequeue(int index)
        {
            if (Items.ContainsKey(index))
                Items.TryRemove(index, out LibraryItem item);
            while (index < Items.Count)
            {
                var successful = Items.TryRemove(++index, out LibraryItem removed);
                if (successful)
                    Items[index - 1] = removed;
            }
            if (CurrentIndex > Items.Count - 1)
                CurrentIndex = 0;
        }

        public KeyValuePair<int, LibraryItem> Forward()
        {
            CurrentIndex++;
            if (Items.Count < 1)
                CurrentIndex = -1;
            else if (CurrentIndex >= Items.Count - 1)
                CurrentIndex = 0;
            return new KeyValuePair<int, LibraryItem>(CurrentIndex, Items[CurrentIndex]);
        }

        public KeyValuePair<int, LibraryItem> Current()
        {
            if (Items.Count > 0)
                return new KeyValuePair<int, LibraryItem>(CurrentIndex, Items[CurrentIndex]);
            return new KeyValuePair<int, LibraryItem>(-1, null);
        }

        public void Clear()
        {
            Items = new ConcurrentDictionary<int, LibraryItem>();
            CurrentIndex = -1;

        }

        public void Reset()
        {
            if (Items.Count > 0)
                CurrentIndex = 0;
            else
                CurrentIndex = -1;
        }
    }
}