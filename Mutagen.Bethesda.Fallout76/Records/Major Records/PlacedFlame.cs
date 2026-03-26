using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout76;

partial class PlacedFlameBinaryOverlay
{
    public IFormLinkGetter<IProjectileGetter> Projectile { get; internal set; } = FormLink<IProjectileGetter>.Null;
}