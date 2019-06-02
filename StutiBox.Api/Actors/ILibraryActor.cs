using System;
using System.Collections.Generic;
using StutiBox.Api.Models;

namespace StutiBox.Api.Actors
{
    public interface ILibraryActor
    {
        List<LibraryItem> LibraryItems { get; }
		DateTime RefreshedAt { get; }
        List<LibraryItem> Find(params string[] keywords);
        LibraryItem LuckySearch(params string[] keywords);
        LibraryItem GetItem(int id);
		bool Refresh();
    }
}