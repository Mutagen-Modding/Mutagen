using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records are allowed as an owner
    /// Implemented by: [Faction, PlacedNpc]
    /// </summary>
    public interface IOwner : IMajorRecordCommon, IOwnerGetter
    {
    }

    /// <summary>
    /// Used for specifying which records are allowed as an owner
    /// Implemented by: [Faction, PlacedNpc]
    /// </summary>
    public interface IOwnerGetter : IMajorRecordCommonGetter
    {
    }
}
