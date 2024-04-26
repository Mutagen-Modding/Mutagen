using System.Collections.Generic;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Implicit;
using Mutagen.Bethesda.Fallout3;

namespace Mutagen.Bethesda
{
    public static class ImplicitsMixIn
    {
        public static IReadOnlyCollection<ModKey> Fallout3(this ImplicitBaseMasters _)
        {
            return Implicits.Get(GameRelease.Fallout3).BaseMasters;
        }

        public static IReadOnlyCollection<ModKey> Fallout3(this ImplicitListings _)
        {
            return Implicits.Get(GameRelease.Fallout3).Listings;
        }

        public static IReadOnlyCollection<FormKey> Fallout3(this ImplicitRecordFormKeys _)
        {
            return Implicits.Get(GameRelease.Fallout3).RecordFormKeys;
        }
    }
}
