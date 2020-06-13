using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// An interface for an object that can be placed in a Cell/Worldspace
    /// Implemented by: [PlacedObject, APlacedTrap]
    /// </summary>
    public interface IPlacedThing : IPlacedThingGetter, IMajorRecordInternal
    {
    }

    /// <summary>
    /// An interface for an object that can be placed in a Cell/Worldspace
    /// Implemented by: [PlacedObject, APlacedTrap]
    /// </summary>
    public interface IPlacedThingGetter : IMajorRecordGetter
    {
    }
}
