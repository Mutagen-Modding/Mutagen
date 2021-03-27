using System.Collections.Generic;
using Mutagen.Bethesda.Oblivion;

namespace Mutagen.Bethesda
{
    public static class ImplicitsMixIn
    {
        public static IReadOnlyCollection<ModKey> Oblivion(this ImplicitBaseMasters _)
        {
            return Implicits.Get(GameRelease.Oblivion).BaseMasters;
        }

        public static IReadOnlyCollection<ModKey> Oblivion(this ImplicitListings _)
        {
            return Implicits.Get(GameRelease.Oblivion).Listings;
        }

        public static IReadOnlyCollection<FormKey> Oblivion(this ImplicitRecordFormKeys _)
        {
            return Implicits.Get(GameRelease.Oblivion).RecordFormKeys;
        }
    }
}
