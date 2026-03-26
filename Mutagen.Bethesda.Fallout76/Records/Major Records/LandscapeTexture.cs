using Mutagen.Bethesda.Plugins;
using System;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout76;

partial class LandscapeTexture
{
    [Flags]
    public enum MajorFlag
    {
    }
}

partial class LandscapeTextureBinaryOverlay
{
    public IReadOnlyList<IFormLinkGetter<IGrassGetter>> Unused => Array.Empty<IFormLinkGetter<IGrassGetter>>();
}
