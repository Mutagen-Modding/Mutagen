using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

partial class PlacedFlameBinaryOverlay
{
    public IFormLinkGetter<IProjectileGetter> Projectile { get; internal set; } = FormLink<IProjectileGetter>.Null;
}