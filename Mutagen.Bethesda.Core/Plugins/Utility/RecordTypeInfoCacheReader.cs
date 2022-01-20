using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Plugins.Analysis;

namespace Mutagen.Bethesda.Plugins.Utility
{
    /// <summary>
    /// A class that can query and cache record locations by record type
    /// </summary>
    public class RecordTypeInfoCacheReader
    {
        private readonly Func<IMutagenReadStream> _streamCreator;
        private readonly Dictionary<Type, HashSet<FormKey>> _cachedLocs = new();

        public RecordTypeInfoCacheReader(Func<IMutagenReadStream> streamCreator)
        {
            this._streamCreator = streamCreator;
        }

        public bool IsOfRecordType<T>(FormKey formKey)
            where T : IMajorRecordGetter
        {
            if (formKey.IsNull) return false;
            lock (_cachedLocs)
            {
                if (!_cachedLocs.TryGetValue(typeof(T), out var cache))
                {
                    using var stream = _streamCreator();
                    var locs = RecordLocator.GetLocations(
                        stream,
                        new RecordInterest(
                            interestingTypes: PluginUtilityTranslation.GetRecordType<T>()));
                    cache = locs.FormKeys.ToHashSet();

                    _cachedLocs.Add(typeof(T), cache);
                }
                return cache.Contains(formKey);
            }
        }
    }
}
