using System;

namespace Mutagen.Bethesda
{
    public static partial class GameCategoryHelper
    {
        public static GameCategory FromModType<TMod>()
            where TMod : IModGetter
        {
            switch (typeof(TMod).Name)
            {
                case "IOblivionMod":
                case "IOblivionGetterMod":
                    return GameCategory.Oblivion;
                case "ISkyrimMod":
                case "ISkyrimGetterMod":
                    return GameCategory.Skyrim;
                default:
                {
                    throw new ArgumentException($"Unknown game type for: {typeof(TMod).Name}");
                }
            }
        }
    }
}
