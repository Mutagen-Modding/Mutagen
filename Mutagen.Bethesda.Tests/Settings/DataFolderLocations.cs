using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Tests
{
    public partial class DataFolderLocations
    {
        public string Get(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Oblivion:
                    return this.Oblivion;
                case GameMode.Skyrim:
                    return this.Skyrim;
                case GameMode.SkyrimSpecialEdition:
                    return this.SkyrimSpecialEdition;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
