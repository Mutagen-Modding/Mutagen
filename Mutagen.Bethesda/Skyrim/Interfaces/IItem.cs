using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records are allowed in a container
    /// Implemented by: [Armor, Ammunition, AlchemicalApparatus, MiscItem, Weapon, Book, LeveledItem, 
    /// Key, Ingestible, Ingredient, Light, Soulgem, Scroll]
    /// </summary>
    public interface IItem : ISkyrimMajorRecordInternal, IItemGetter
    {
    }

    /// <summary>
    /// Used for specifying which records are allowed in a container
    /// Implemented by: [Armor, Ammunition, AlchemicalApparatus, MiscItem, Weapon, Book, LeveledItem, 
    /// Key, Ingestible, Ingredient, Light, Soulgem, Scroll]
    /// </summary>
    public interface IItemGetter : ISkyrimMajorRecordGetter
    {
    }
}
