using Noggog;
using System;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout76;

partial class LeveledPackIn
{
    [Flags]
    public enum MajorFlag
    {
    }
}

partial class LeveledPackInBinaryOverlay
{
    public ReadOnlyMemorySlice<Byte> BelowVersion => Array.Empty<byte>();
}
