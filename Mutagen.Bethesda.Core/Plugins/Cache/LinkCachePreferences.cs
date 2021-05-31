using System;

namespace Mutagen.Bethesda.Plugins.Cache
{
    public abstract class LinkCachePreferences
    {
        public static LinkCachePreferences Default => WholeRecord();

        public static LinkCachePreferences WholeRecord() => LinkCachePreferenceWholeRecord.Instance;

        public static LinkCachePreferences OnlyIdentifiers() => LinkCachePreferenceOnlyIdentifiers
            .Instance;
    }
}
