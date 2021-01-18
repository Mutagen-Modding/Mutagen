using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class GameReleaseExt
    {
        public static GameCategory ToCategory(this GameRelease release)
        {
            return release switch
            {
                GameRelease.Oblivion => GameCategory.Oblivion,
                GameRelease.SkyrimLE => GameCategory.Skyrim,
                GameRelease.SkyrimSE => GameCategory.Skyrim,
                GameRelease.SkyrimVR => GameCategory.Skyrim,
                GameRelease.Fallout4 => GameCategory.Fallout4,
                _ => throw new NotImplementedException(),
            };
        }

        public static ushort? GetDefaultFormVersion(this GameRelease release)
        {
            return release switch
            {
                GameRelease.Oblivion => default,
                GameRelease.SkyrimLE => 43,
                GameRelease.SkyrimSE => 44,
                GameRelease.SkyrimVR => 44,
                GameRelease.Fallout4 => default,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
