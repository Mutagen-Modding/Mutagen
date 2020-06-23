using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records are allowed as something that can be retrieved from a harvest action.
    /// Implemented by: [Ingredient, Ingestible, LeveledItem, MiscItem]
    /// </summary>
    public interface IHarvestTarget : IHarvestTargetGetter, ISkyrimMajorRecordInternal
    {
    }

    /// <summary>
    /// Used for specifying which records are allowed as something that can be retrieved from a harvest action.
    /// Implemented by: [Ingredient, Ingestible, LeveledItem, MiscItem]
    /// </summary>
    public interface IHarvestTargetGetter : ISkyrimMajorRecordGetter
    {
    }
}
