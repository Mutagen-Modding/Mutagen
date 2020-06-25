using Mutagen.Bethesda;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Implemented by: [SoundDescriptor, SoundMarker]
    /// </summary>
    public partial interface ISound :
        ISkyrimMajorRecordInternal,
        ISoundGetter
    {
    }

    /// <summary>
    /// Implemented by: [SoundDescriptor, SoundMarker]
    /// </summary>
    public partial interface ISoundGetter : ISkyrimMajorRecordGetter
    {
    }
}
