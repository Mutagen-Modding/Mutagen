using System.Collections.Generic;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Implicit;
using Mutagen.Bethesda.Fallout3;

namespace Mutagen.Bethesda
{
    public static class ImplicitsMixIn
    {
        public static IReadOnlyCollection<ModKey> Fallout3(
            this ImplicitBaseMasters _,
            Fallout3Release release)
        {
            return Implicits.Get(release.ToGameRelease()).BaseMasters;
        }

        public static IReadOnlyCollection<ModKey> Fallout3(
            this ImplicitListings _,
            Fallout3Release release)
        {
            return Implicits.Get(release.ToGameRelease()).Listings;
        }

        public static IReadOnlyCollection<FormKey> Fallout3(
            this ImplicitRecordFormKeys _,
            Fallout3Release release)
        {
            return Implicits.Get(release.ToGameRelease()).RecordFormKeys;
        }
    }
}
