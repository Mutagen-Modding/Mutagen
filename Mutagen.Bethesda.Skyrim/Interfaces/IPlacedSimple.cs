using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Implemented by: [PlacedObject, PlacedNpc]
    /// </summary>
    public interface IPlacedSimple : IPlacedSimpleGetter, IMajorRecordInternal
    {
    }

    /// <summary>
    /// Implemented by: [PlacedObject, PlacedNpc]
    /// </summary>
    public interface IPlacedSimpleGetter : IMajorRecordGetter
    {
    }
}
