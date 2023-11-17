using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

partial class PlacedHazardBinaryOverlay
{
    public IFormLinkGetter<IHazardGetter> Hazard { get; internal set; } = FormLink<IHazardGetter>.Null;
}