using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Implemented by: [Cell, Worldspace]
    /// </summary>
    public interface IComplexLocation : ISkyrimMajorRecordInternal, IComplexLocationGetter
    {
    }

    /// <summary>
    /// Implemented by: [Cell, Worldspace]
    /// </summary>
    public interface IComplexLocationGetter : ISkyrimMajorRecordGetter
    {
    }
}
