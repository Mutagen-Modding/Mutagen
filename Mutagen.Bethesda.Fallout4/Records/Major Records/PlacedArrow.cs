using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4;

partial class PlacedArrowBinaryOverlay
{
    public IFormLinkGetter<IProjectileGetter> Projectile { get; internal set; } = FormLink<IProjectileGetter>.Null;
}