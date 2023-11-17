using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

partial class PlacedBeamBinaryOverlay
{
    public IFormLinkGetter<IProjectileGetter> Projectile { get; internal set; } = FormLink<IProjectileGetter>.Null;
}