using Loqui;
using System;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Plugins.Records.Internals;

public static class GameCategoryExt
{
    public static ILoquiRegistration ToModRegistration(this GameCategory category)
    {
        return category switch
        {
            GameCategory.Oblivion => OblivionMod.StaticRegistration,
            GameCategory.Skyrim => SkyrimMod.StaticRegistration,
            GameCategory.Fallout4 => Fallout4Mod.StaticRegistration,
            _ => throw new NotImplementedException(),
        };
    }
}