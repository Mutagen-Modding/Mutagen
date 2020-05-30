using Loqui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    /// <summary>
    /// An interface for something that can be placed in a Cell/Worldspace
    /// Implemented by: [Landscape, PlacedCreature, PlacedNpc, PlacedObject]
    /// </summary>
    public interface IPlaced : IMajorRecordInternal, IPlacedGetter
    {
    }

    /// <summary>
    /// A getter interface for something that can be placed in a Cell/Worldspace
    /// Implemented by: [Landscape, PlacedCreature, PlacedNpc, PlacedObject]
    /// </summary>
    public interface IPlacedGetter : IMajorRecordGetter, ILoquiObjectGetter
    {
    }
}
