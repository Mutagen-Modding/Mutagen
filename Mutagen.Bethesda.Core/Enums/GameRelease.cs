using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// Specific game releases
    /// </summary>
    public enum GameRelease
    {
        Oblivion,
        Skyrim,
        SkyrimSpecialEdition
    }

    public static class GameReleaseExt
    {
        public static GameCategory ToCategory(this GameRelease release)
        {
            return release switch
            {
                GameRelease.Oblivion => GameCategory.Oblivion,
                GameRelease.Skyrim => GameCategory.Skyrim,
                GameRelease.SkyrimSpecialEdition => GameCategory.Skyrim,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
