using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

partial class PlacedFlameBinaryOverlay
{
    public IFormLinkGetter<IProjectileGetter> Projectile { get; internal set; } = FormLink<IProjectileGetter>.Null;
}