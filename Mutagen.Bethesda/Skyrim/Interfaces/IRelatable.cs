using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records can be specified in a Relation
    /// Implemented by: [Faction, Race]
    /// </summary>
    public interface IRelatable : ISkyrimMajorRecord, IRelatableGetter
    {
    }

    /// <summary>
    /// Used for specifying which records can be specified in a Relation
    /// Implemented by: [Faction, Race]
    /// </summary>
    public interface IRelatableGetter : ISkyrimMajorRecordGetter
    {
    }
}
