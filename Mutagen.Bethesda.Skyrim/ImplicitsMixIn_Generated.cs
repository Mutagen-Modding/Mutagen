using System.Collections.Generic;
using Mutagen.Bethesda.Implicit;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda
{
    public static class ImplicitsMixIn
    {
        public static IReadOnlyCollection<ModKey> Skyrim(
            this ImplicitBaseMasters _,
            SkyrimRelease release)
        {
            return Implicits.Get(release.ToGameRelease()).BaseMasters;
        }

        public static IReadOnlyCollection<ModKey> Skyrim(
            this ImplicitListings _,
            SkyrimRelease release)
        {
            return Implicits.Get(release.ToGameRelease()).Listings;
        }

        public static IReadOnlyCollection<FormKey> Skyrim(
            this ImplicitRecordFormKeys _,
            SkyrimRelease release)
        {
            return Implicits.Get(release.ToGameRelease()).RecordFormKeys;
        }
    }
}
