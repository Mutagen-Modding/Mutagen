using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records are allowed as something that can be retrieved from a harvest action
    /// </summary>
    public interface IHarvestTarget : IHarvestTargetGetter, IMajorRecordCommon
    {
    }

    /// <summary>
    /// Used for specifying which records are allowed as something that can be retrieved from a harvest action
    /// </summary>
    public interface IHarvestTargetGetter : IMajorRecordCommonGetter
    {
    }
}
