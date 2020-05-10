using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Internals
{
    /// <summary>
    /// A class that can query and cache record locations by record type
    /// </summary>
    public class RecordInfoCache
    {
        private readonly Func<IMutagenReadStream> _streamCreator;
        private readonly Dictionary<Type, HashSet<FormID>> _cachedLocs = new Dictionary<Type, HashSet<FormID>>();

        public RecordInfoCache(Func<IMutagenReadStream> streamCreator)
        {
            this._streamCreator = streamCreator;
        }

        public bool IsOfRecordType<T>(FormID formID)
            where T : IMajorRecordCommonGetter
        {
            if (formID == FormID.Null) return false;
            lock (_cachedLocs)
            {
                if (!_cachedLocs.TryGetValue(typeof(T), out var cache))
                {
                    using var stream = _streamCreator();
                    var locs = RecordLocator.GetFileLocations(
                        stream,
                        new RecordInterest(
                            interestingTypes: UtilityTranslation.GetRecordType<T>()));
                    cache = locs.FormIDs.ToHashSet();

                    _cachedLocs.Add(typeof(T), cache);
                }
                return cache.Contains(formID);
            }
        }
    }
}
