using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records are sounds
    /// Implemented by: [SoundDescriptor, SoundMarker]
    /// </summary>
    public interface ISound : IMajorRecordCommon, ISoundGetter
    {
    }

    /// <summary>
    /// Used for specifying which records are sounds
    /// Implemented by: [SoundDescriptor, SoundMarker]
    /// </summary>
    public interface ISoundGetter : IMajorRecordCommonGetter
    {
    }
}
