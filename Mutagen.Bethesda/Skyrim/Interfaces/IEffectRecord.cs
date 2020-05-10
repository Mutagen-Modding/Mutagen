using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records are allowed as an Object Effect
    /// Implemented by: [Spell, ObjectEffect]
    /// </summary>
    public interface IEffectRecord : ISkyrimMajorRecordInternal, IEffectRecordGetter
    {
    }

    /// <summary>
    /// Used for specifying which records are allowed as an Object Effect
    /// Implemented by: [Spell, ObjectEffect]
    /// </summary>
    public interface IEffectRecordGetter : ISkyrimMajorRecordGetter
    {
    }
}
