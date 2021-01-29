using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class GameCategoryExt
    {
        public static bool HasFormVersion(this GameCategory release)
        {
            return release switch
            {
                GameCategory.Oblivion => false,
                GameCategory.Skyrim => true,
                GameCategory.Fallout4 => true,
                _ => throw new NotImplementedException(),
            };
        }

        public static GameRelease DefaultRelease(this GameCategory gameCategory)
        {
            return gameCategory switch
            {
                GameCategory.Oblivion => GameRelease.Oblivion,
                GameCategory.Skyrim => GameRelease.SkyrimSE,
                GameCategory.Fallout4 => GameRelease.Fallout4,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
