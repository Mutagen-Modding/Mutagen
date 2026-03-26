using Noggog;
using System;

namespace Mutagen.Bethesda.Fallout76;

partial class IdleMarker
{
    [Flags]
    public enum MajorFlag
    {
        ChildCanUse = 0x2000_0000,
    }
}

partial class IdleMarkerBinaryOverlay
{
    public ReadOnlyMemorySlice<Byte> IsFO3 => Array.Empty<byte>();
    public ReadOnlyMemorySlice<Byte> IsSF1 => Array.Empty<byte>();
}
