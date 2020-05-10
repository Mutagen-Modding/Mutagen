using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records are allowed as an Object Effect
    /// </summary>
    public interface IEffectRecord : ISkyrimMajorRecordInternal, IEffectRecordGetter
    {
    }

    /// <summary>
    /// Used for specifying which records are allowed as an Object Effect
    /// </summary>
    public interface IEffectRecordGetter : ISkyrimMajorRecordGetter
    {
    }
}
