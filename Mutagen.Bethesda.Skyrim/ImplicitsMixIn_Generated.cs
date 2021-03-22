using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda
{
    public static class ImplicitsMixIn
    {
        public static ICollection<ModKey> Skyrim(
            this ImplicitBaseMasters _,
            SkyrimRelease release)
        {
            return Implicits.Get(release.ToGameRelease()).BaseMasters;
        }

        public static ICollection<ModKey> Skyrim(
            this ImplicitListings _,
            SkyrimRelease release)
        {
            return Implicits.Get(release.ToGameRelease()).Listings;
        }

        public static ICollection<FormKey> Skyrim(
            this ImplicitRecordFormKeys _,
            SkyrimRelease release)
        {
            return Implicits.Get(release.ToGameRelease()).RecordFormKeys;
        }
    }
}
