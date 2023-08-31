using System.Collections.Generic;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Implicit;
using Mutagen.Bethesda.Starfield;

namespace Mutagen.Bethesda
{
    public static class ImplicitsMixIn
    {
        public static IReadOnlyCollection<ModKey> Starfield(this ImplicitBaseMasters _)
        {
            return Implicits.Get(GameRelease.Starfield).BaseMasters;
        }

        public static IReadOnlyCollection<ModKey> Starfield(this ImplicitListings _)
        {
            return Implicits.Get(GameRelease.Starfield).Listings;
        }

        public static IReadOnlyCollection<FormKey> Starfield(this ImplicitRecordFormKeys _)
        {
            return Implicits.Get(GameRelease.Starfield).RecordFormKeys;
        }
    }
}
