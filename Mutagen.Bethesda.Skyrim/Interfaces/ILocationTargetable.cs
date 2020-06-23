using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records can be specified in a Location Reference.
    /// Implemented by: [Door, PlacedNpc, PlacedObject]
    /// </summary>
    public interface ILocationTargetable : IMajorRecordCommon, ILocationTargetableGetter
    {
    }

    /// <summary>
    /// Used for specifying which records can be specified in a Location Reference.
    /// Implemented by: [Door, PlacedNpc, PlacedObject]
    /// </summary>
    public interface ILocationTargetableGetter : IMajorRecordCommonGetter
    {
    }

    // ToDo
    // Add placed traps, and Player
}
