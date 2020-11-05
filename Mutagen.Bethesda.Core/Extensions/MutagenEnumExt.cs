using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class MutagenEnumExt
    {
        public static GameCategory ToCategory(this GameRelease release)
        {
            return release switch
            {
                GameRelease.Oblivion => GameCategory.Oblivion,
                GameRelease.SkyrimLE => GameCategory.Skyrim,
                GameRelease.SkyrimSE => GameCategory.Skyrim,
                GameRelease.SkyrimVR => GameCategory.Skyrim,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
