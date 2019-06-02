using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StutiBox.Api.Config
{
    public class LibraryConfiguration
    {
        public string MusicDirectory { get; set; }
        public string ScanForUpdates { get; set; }
    }
}