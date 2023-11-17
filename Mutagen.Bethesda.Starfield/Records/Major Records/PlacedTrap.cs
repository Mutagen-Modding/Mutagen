using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

partial class PlacedTrapBinaryOverlay
{
    public IFormLinkGetter<IProjectileGetter> Projectile { get; internal set; } = FormLink<IProjectileGetter>.Null;
}