using Loqui;
using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Skyrim.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class GameCategoryExt
    {
        public static ILoquiRegistration ToModRegistration(this GameCategory category)
        {
            return category switch
            {
                GameCategory.Oblivion => OblivionMod_Registration.Instance,
                GameCategory.Skyrim => SkyrimMod_Registration.Instance,
                _ => throw new NotImplementedException(),
            };
        }

        public static GameRelease DefaultRelease(this GameCategory gameCategory)
        {
            return gameCategory switch
            {
                GameCategory.Oblivion => GameRelease.Oblivion,
                GameCategory.Skyrim => GameRelease.SkyrimSE,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
