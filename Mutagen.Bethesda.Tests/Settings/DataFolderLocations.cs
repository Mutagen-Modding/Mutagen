using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Tests
{
    public partial class DataFolderLocations
    {
        public string Get(GameRelease mode)
        {
            switch (mode)
            {
                case GameRelease.Oblivion:
                    return this.Oblivion;
                case GameRelease.Skyrim:
                    return this.Skyrim;
                case GameRelease.SkyrimSpecialEdition:
                    return this.SkyrimSpecialEdition;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
