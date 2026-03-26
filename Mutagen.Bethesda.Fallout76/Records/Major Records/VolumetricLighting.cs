using System;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout76;

partial class VolumetricLighting
{
    [Flags]
    public enum MajorFlag
    {
    }
}

partial class VolumetricLightingBinaryOverlay
{
    public Single Blue => 0f;
    public Single Green => 0f;
    public Single Red => 0f;
}
