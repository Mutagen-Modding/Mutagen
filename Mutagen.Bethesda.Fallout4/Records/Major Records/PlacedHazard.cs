using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4;

partial class PlacedHazardBinaryOverlay
{
    public IFormLinkGetter<IHazardGetter> Hazard { get; internal set; } = FormLink<IHazardGetter>.Null;
}