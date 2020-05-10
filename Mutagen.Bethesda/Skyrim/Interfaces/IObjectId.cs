using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records can be specified in a Location ObjectID.
    /// Implemented by: [Activator, Door, Static, MoveableStatic, Furniture, Spell, Scroll, Npc, Container, Armor, 
    /// Ammunition, MiscItem, Weapon, Book, Key, Ingestible, Light, Faction, FormList, IdleMarker, Shout]
    /// </summary>
    public interface IObjectId : IMajorRecordCommon, IObjectIdGetter
    {
    }

    /// <summary>
    /// Used for specifying which records can be specified in a Location ObjectID.
    /// Implemented by: [Activator, Door, Static, MoveableStatic, Furniture, Spell, Scroll, Npc, Container, Armor, 
    /// Ammunition, MiscItem, Weapon, Book, Key, Ingestible, Light, Faction, FormList, IdleMarker, Shout]
    /// </summary>
    public interface IObjectIdGetter : IMajorRecordCommonGetter
    {
    }
}
