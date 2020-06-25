using Mutagen.Bethesda;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Implemented by: [AlchemicalApparatus, Ammunition, Armor, Book, Ingestible, Ingredient, Key, LeveledItem, Light, MiscItem, Scroll, SoulGem, Weapon]
    /// </summary>
    public partial interface IItem :
        ISkyrimMajorRecordInternal,
        IItemGetter
    {
    }

    /// <summary>
    /// Implemented by: [AlchemicalApparatus, Ammunition, Armor, Book, Ingestible, Ingredient, Key, LeveledItem, Light, MiscItem, Scroll, SoulGem, Weapon]
    /// </summary>
    public partial interface IItemGetter : ISkyrimMajorRecordGetter
    {
    }
}
