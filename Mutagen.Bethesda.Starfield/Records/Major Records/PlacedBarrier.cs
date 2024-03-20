using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

partial class PlacedBarrierBinaryOverlay
{
    public IFormLinkGetter<IProjectileGetter> Projectile { get; internal set; } = FormLink<IProjectileGetter>.Null;
}