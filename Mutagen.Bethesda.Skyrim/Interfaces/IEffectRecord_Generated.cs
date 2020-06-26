using Mutagen.Bethesda;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Implemented by: [ObjectEffect, Spell]
    /// </summary>
    public partial interface IEffectRecord :
        ISkyrimMajorRecordInternal,
        IEffectRecordGetter
    {
    }

    /// <summary>
    /// Implemented by: [ObjectEffect, Spell]
    /// </summary>
    public partial interface IEffectRecordGetter : ISkyrimMajorRecordGetter
    {
    }
}
