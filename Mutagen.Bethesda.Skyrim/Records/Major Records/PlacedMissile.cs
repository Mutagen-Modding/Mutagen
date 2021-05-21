using System;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PlacedMissileBinaryOverlay
        {
            public IFormLinkGetter<IProjectileGetter> Projectile { get; internal set; } = FormLink<IProjectileGetter>.Null;
        }
    }
}