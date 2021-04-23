using System.Collections.Generic;
using Mutagen.Bethesda.Implicit;
using Mutagen.Bethesda.Fallout4;

namespace Mutagen.Bethesda
{
    public static class ImplicitsMixIn
    {
        public static IReadOnlyCollection<ModKey> Fallout4(this ImplicitBaseMasters _)
        {
            return Implicits.Get(GameRelease.Fallout4).BaseMasters;
        }

        public static IReadOnlyCollection<ModKey> Fallout4(this ImplicitListings _)
        {
            return Implicits.Get(GameRelease.Fallout4).Listings;
        }

        public static IReadOnlyCollection<FormKey> Fallout4(this ImplicitRecordFormKeys _)
        {
            return Implicits.Get(GameRelease.Fallout4).RecordFormKeys;
        }
    }
}
