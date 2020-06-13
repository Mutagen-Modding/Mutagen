using System;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PlacedFlameBinaryOverlay
        {
            public IFormLink<IProjectileGetter> Projectile { get; internal set; } = FormLink<IProjectileGetter>.Null;
        }
    }
}