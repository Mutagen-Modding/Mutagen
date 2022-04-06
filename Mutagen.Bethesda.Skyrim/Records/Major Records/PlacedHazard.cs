using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

partial class PlacedHazardBinaryOverlay
{
    public IFormLinkGetter<IHazardGetter> Hazard { get; internal set; } = FormLink<IHazardGetter>.Null;
}