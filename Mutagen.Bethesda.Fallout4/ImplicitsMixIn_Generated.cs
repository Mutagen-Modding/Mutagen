using System.Collections.Generic;
using Mutagen.Bethesda.Fallout4;

namespace Mutagen.Bethesda
{
    public static class ImplicitsMixIn
    {
        public static ICollection<ModKey> Fallout4(this ImplicitBaseMasters _)
        {
            return Implicits.Get(GameRelease.Fallout4).BaseMasters;
        }

        public static ICollection<ModKey> Fallout4(this ImplicitListings _)
        {
            return Implicits.Get(GameRelease.Fallout4).Listings;
        }

        public static ICollection<FormKey> Fallout4(this ImplicitRecordFormKeys _)
        {
            return Implicits.Get(GameRelease.Fallout4).RecordFormKeys;
        }
    }
}
