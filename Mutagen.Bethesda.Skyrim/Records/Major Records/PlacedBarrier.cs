using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PlacedBarrierBinaryOverlay
        {
            public IFormLinkGetter<IProjectileGetter> Projectile { get; internal set; } = FormLink<IProjectileGetter>.Null;
        }
    }
}
