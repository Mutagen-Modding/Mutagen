using System.Collections.Generic;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Implicit;
using Mutagen.Bethesda.Oblivion;

namespace Mutagen.Bethesda
{
    public static class ImplicitsMixIn
    {
        public static IReadOnlyCollection<ModKey> Oblivion(
            this ImplicitBaseMasters _,
            OblivionRelease release)
        {
            return Implicits.Get(release.ToGameRelease()).BaseMasters;
        }

        public static IReadOnlyCollection<ModKey> Oblivion(
            this ImplicitListings _,
            OblivionRelease release)
        {
            return Implicits.Get(release.ToGameRelease()).Listings;
        }

        public static IReadOnlyCollection<FormKey> Oblivion(
            this ImplicitRecordFormKeys _,
            OblivionRelease release)
        {
            return Implicits.Get(release.ToGameRelease()).RecordFormKeys;
        }
    }
}
