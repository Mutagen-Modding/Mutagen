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
        SkyrimLE,
        SkyrimSE
    }

    public static class GameReleaseExt
    {
        public static GameCategory ToCategory(this GameRelease release)
        {
            return release switch
            {
                GameRelease.Oblivion => GameCategory.Oblivion,
                GameRelease.SkyrimLE => GameCategory.Skyrim,
                GameRelease.SkyrimSE => GameCategory.Skyrim,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
