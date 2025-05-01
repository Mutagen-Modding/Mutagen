using System.Collections.Generic;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Implicit;
using Mutagen.Bethesda.Morrowind;

namespace Mutagen.Bethesda
{
    public static class ImplicitsMixIn
    {
        public static IReadOnlyCollection<ModKey> Morrowind(this ImplicitBaseMasters _)
        {
            return Implicits.Get(GameRelease.Morrowind).BaseMasters;
        }

        public static IReadOnlyCollection<ModKey> Morrowind(this ImplicitListings _)
        {
            return Implicits.Get(GameRelease.Morrowind).Listings;
        }

        public static IReadOnlyCollection<FormKey> Morrowind(this ImplicitRecordFormKeys _)
        {
            return Implicits.Get(GameRelease.Morrowind).RecordFormKeys;
        }
    }
}
