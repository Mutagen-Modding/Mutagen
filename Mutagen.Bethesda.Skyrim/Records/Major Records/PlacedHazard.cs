using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PlacedHazardBinaryOverlay
        {
            public IFormLinkGetter<IHazardGetter> Hazard { get; internal set; } = FormLink<IHazardGetter>.Null;
        }
    }
}