using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Common interface for records that can be emitted
    /// Implemented by: [Light, Region]
    /// </summary>
    public interface IEmittance : IMajorRecordCommon, IEmittanceGetter
    {
    }

    /// <summary>
    /// Common interface for records that can be emitted
    /// Implemented by: [Light, Region]
    /// </summary>
    public interface IEmittanceGetter : IMajorRecordCommonGetter
    {
    }
}
