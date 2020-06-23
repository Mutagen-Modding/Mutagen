using System;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PlacedHazardBinaryOverlay
        {
            public IFormLink<IHazardGetter> Hazard { get; internal set; } = FormLink<IHazardGetter>.Null;
        }
    }
}