using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records can be specified in a Location Reference.
    /// Implemented by: [Door, PlacedNpc, PlacedObject]
    /// </summary>
    public interface ILocationReferencable : IMajorRecordCommon, ILocationReferencableGetter
    {
    }

    /// <summary>
    /// Used for specifying which records can be specified in a Location Reference.
    /// Implemented by: [Door, PlacedNpc, PlacedObject]
    /// </summary>
    public interface ILocationReferencableGetter : IMajorRecordCommonGetter
    {
    }

    // ToDo
    // Add placed traps, and Player
}
