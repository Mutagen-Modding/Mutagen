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
                _ => throw new NotImplementedException(),
            };
        }
    }
}
