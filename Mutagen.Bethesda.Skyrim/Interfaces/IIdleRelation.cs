using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records are allowed as a related idle
    /// Implemented by: [IdleAnimation, ActionRecord]
    /// </summary>
    public interface IIdleRelation : IMajorRecordCommon, IIdleRelationGetter
    {
    }

    /// <summary>
    /// Used for specifying which records are allowed as a related idle
    /// Implemented by: [IdleAnimation, ActionRecord]
    /// </summary>
    public interface IIdleRelationGetter : IMajorRecordCommonGetter
    {
    }
}
