using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Common interface for Location related records
    /// Implemented by: [Location, LocationReferenceType]
    /// </summary>
    public interface ILocationRecord : ILocationRecordGetter, IMajorRecordCommon
    {
    }

    /// <summary>
    /// Common interface for Location related records
    /// Implemented by: [Location, LocationReferenceType]
    /// </summary>
    public interface ILocationRecordGetter : IMajorRecordCommonGetter
    {
    }
}
