using Noggog;
using System;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout76;

partial class LeveledPerkCard
{
    [Flags]
    public enum MajorFlag
    {
    }
}

partial class LeveledPerkCardBinaryOverlay
{
    public ReadOnlyMemorySlice<Byte> BelowVersion => Array.Empty<byte>();
}
