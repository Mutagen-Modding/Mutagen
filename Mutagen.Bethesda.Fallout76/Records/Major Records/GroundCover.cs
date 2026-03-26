using Mutagen.Bethesda.Plugins;
using System;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout76;

partial class GroundCover
{
    [Flags]
    public enum MajorFlag
    {
    }
}

partial class GroundCoverBinaryOverlay
{
    public IReadOnlyList<IFormLinkGetter<ILandscapeTextureGetter>> LandscapeTextures => Array.Empty<IFormLinkGetter<ILandscapeTextureGetter>>();
}
