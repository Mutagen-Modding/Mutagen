using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

partial class PlacedArrowBinaryOverlay
{
    public IFormLinkGetter<IProjectileGetter> Projectile { get; internal set; } = FormLink<IProjectileGetter>.Null;
}