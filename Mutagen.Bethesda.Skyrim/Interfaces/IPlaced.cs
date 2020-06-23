using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// An interface for something that can be placed in a Cell/Worldspace
    /// Implemented by: [PlacedObject, PlacedNpc, APlacedTrap]
    /// </summary>
    public interface IPlaced : IPlacedGetter, IMajorRecordInternal, IPlacedThing, IPlacedSimple
    {
    }

    /// <summary>
    /// An interface for something that can be placed in a Cell/Worldspace
    /// Implemented by: [PlacedObject, PlacedNpc, APlacedTrap]
    /// </summary>
    public interface IPlacedGetter : IMajorRecordGetter, IPlacedThingGetter, IPlacedSimpleGetter
    {
    }
}
