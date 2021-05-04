using Loqui;
using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Fallout4.Internals;
using System;

namespace Mutagen.Bethesda.Plugins.Records.Internals
{
    public static class GameCategoryExt
    {
        public static ILoquiRegistration ToModRegistration(this GameCategory category)
        {
            return category switch
            {
                GameCategory.Oblivion => OblivionMod_Registration.Instance,
                GameCategory.Skyrim => SkyrimMod_Registration.Instance,
                GameCategory.Fallout4 => Fallout4Mod_Registration.Instance,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
