using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout76;

partial class PlacedArrowBinaryOverlay
{
    public IFormLinkGetter<IProjectileGetter> Projectile { get; internal set; } = FormLink<IProjectileGetter>.Null;
}