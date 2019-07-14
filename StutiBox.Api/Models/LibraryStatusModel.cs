using System;
using StutiBox.Api.Actors;
using System.Collections.Generic;
namespace StutiBox.Api.Models
{
    public class LibraryStatusModel
    {
        public bool Status {get;set;}
        public DateTime LibraryRefreshedAt {get;set;}
        public List<LibraryItem> Items {get;set;}
    }
}