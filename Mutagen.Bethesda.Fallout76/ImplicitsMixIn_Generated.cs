using System.Collections.Generic;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Implicit;
using Mutagen.Bethesda.Fallout76;

namespace Mutagen.Bethesda
{
    public static class ImplicitsMixIn
    {
        public static IReadOnlyCollection<ModKey> Fallout76(
            this ImplicitBaseMasters _,
            Fallout76Release release)
        {
            return Implicits.Get(release.ToGameRelease()).BaseMasters;
        }

        public static IReadOnlyCollection<ModKey> Fallout76(
            this ImplicitListings _,
            Fallout76Release release)
        {
            return Implicits.Get(release.ToGameRelease()).Listings;
        }

        public static IReadOnlyCollection<FormKey> Fallout76(
            this ImplicitRecordFormKeys _,
            Fallout76Release release)
        {
            return Implicits.Get(release.ToGameRelease()).RecordFormKeys;
        }
    }
}
