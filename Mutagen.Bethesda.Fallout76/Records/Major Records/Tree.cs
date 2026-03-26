using Noggog;
using System;

namespace Mutagen.Bethesda.Fallout76;

partial class Tree
{
    [Flags]
    public enum MajorFlag
    {
        HasDistantLod = 0x0000_8000
    }
}

partial class TreeBinaryOverlay
{
    public ReadOnlyMemorySlice<Byte> sigBaseObjects => Array.Empty<byte>();
}