using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records can be specified in a Linked Reference
    /// Implemented by: [Player, PlacedNpc, PlacedObject, APlacedTrap]
    /// </summary>
    public interface ILinkedReference : ILinkedReferenceGetter, IMajorRecordCommon, IPlaced
    {
    }

    /// <summary>
    /// Used for specifying which records can be specified in a Linked Reference
    /// Implemented by: [Player, PlacedNpc, PlacedObject, APlacedTrap]
    /// </summary>
    public interface ILinkedReferenceGetter : IMajorRecordCommonGetter, IPlacedGetter
    {
    }
}
