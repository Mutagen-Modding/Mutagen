using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Tests
{
    public partial class Target
    {
        public FilePath GetFilePath(DataFolderLocations locs)
        {
            return new FilePath(System.IO.Path.Combine(locs.Get(this.GameMode), this.Path));
        }
    }
}
