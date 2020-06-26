using Mutagen.Bethesda;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Implemented by: [Light, Region]
    /// </summary>
    public partial interface IEmittance :
        ISkyrimMajorRecordInternal,
        IEmittanceGetter
    {
    }

    /// <summary>
    /// Implemented by: [Light, Region]
    /// </summary>
    public partial interface IEmittanceGetter : ISkyrimMajorRecordGetter
    {
    }
}
