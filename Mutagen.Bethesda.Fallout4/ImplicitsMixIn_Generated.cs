using System.Collections.Generic;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Implicit;
using Mutagen.Bethesda.Fallout4;

namespace Mutagen.Bethesda
{
    public static class ImplicitsMixIn
    {
        public static IReadOnlyCollection<ModKey> Fallout4(
            this ImplicitBaseMasters _,
            Fallout4Release release)
        {
            return Implicits.Get(release.ToGameRelease()).BaseMasters;
        }

        public static IReadOnlyCollection<ModKey> Fallout4(
            this ImplicitListings _,
            Fallout4Release release)
        {
            return Implicits.Get(release.ToGameRelease()).Listings;
        }

        public static IReadOnlyCollection<FormKey> Fallout4(
            this ImplicitRecordFormKeys _,
            Fallout4Release release)
        {
            return Implicits.Get(release.ToGameRelease()).RecordFormKeys;
        }
    }
}
