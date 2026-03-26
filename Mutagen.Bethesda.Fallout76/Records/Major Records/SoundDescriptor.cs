using Mutagen.Bethesda.Plugins;
using System;

namespace Mutagen.Bethesda.Fallout76;

partial class SoundDescriptor
{
    [Flags]
    public enum MajorFlag
    {
    }

    public enum LoopType
    {
        None = 0,
        Loop = 0x08,
        EnvelopeFast = 0x10,
        EnvelopeSlow = 0x20,
    }

    public enum SoundLooping
    {
        None = 0,
        Loop = 1,
        EnvelopeFast = 2,
        EnvelopeSlow = 3,
        Unknown128 = 128,
        Unknown136 = 136,
    }
}

partial class SoundDescriptorBinaryOverlay
{
}
