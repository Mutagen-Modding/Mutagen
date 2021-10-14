using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis
{
    public class RecordLocatorResults
    {
        private readonly Dictionary<FormKey, (RangeInt64 Range, IEnumerable<long> GroupPositions, RecordType Record)>
            _fromFormKeys;

        private readonly SortingListDictionary<long, RecordLocationMarker> _fromStart;
        private readonly SortingListDictionary<long, RecordLocationMarker> _fromEnd;
        private readonly SortingListDictionary<long, long> _grupLocations;
        public ISortedListGetter<long> GrupLocations => _grupLocations.Keys;

        public SortingListDictionary<long, RecordLocationMarker> ListedRecords =>
            _fromStart;

        public RangeInt64 this[FormKey formKey] => _fromFormKeys[formKey].Range;
        public ICollectionGetter<FormKey> FormKeys => new CollectionGetterWrapper<FormKey>(_fromFormKeys.Keys);

        internal RecordLocatorResults(RecordLocator.FileLocationConstructor constructor)
        {
            _fromFormKeys = constructor.FromFormKeys;
            _fromStart = SortingListDictionary<long, RecordLocationMarker>.Factory_Wrap_AssumeSorted(
                constructor.FromStartPositions,
                constructor.FormKeys);
            _fromEnd = SortingListDictionary<long, RecordLocationMarker>.Factory_Wrap_AssumeSorted(
                constructor.FromEndPositions,
                constructor.FormKeys);
            _grupLocations =
                new SortingListDictionary<long, long>(
                    constructor.GrupLocations.Select(i => new KeyValuePair<long, long>(i, i)));
        }

        public bool TryGetSection(FormKey formKey, out RangeInt64 section)
        {
            if (this._fromFormKeys.TryGetValue(formKey, out var item))
            {
                section = item.Range;
                return true;
            }

            section = default(RangeInt64);
            return false;
        }

        public bool TryGetRecord(long loc, [MaybeNullWhen(false)] out RecordLocationMarker record)
        {
            if (!_fromStart.TryGetInDirection(
                key: loc,
                higher: false,
                result: out var lowerKeyRecord))
            {
                record = default;
                return false;
            }

            if (!_fromEnd.TryGetInDirection(
                key: loc,
                higher: true,
                result: out var higherKeyRecord))
            {
                record = default;
                return false;
            }

            if (lowerKeyRecord.Value.FormKey != higherKeyRecord.Value.FormKey)
            {
                record = default;
                return false;
            }

            record = lowerKeyRecord.Value;
            return true;
        }

        public bool TryGetRecords(RangeInt64 section,
            [MaybeNullWhen(false)] out IEnumerable<RecordLocationMarker> records)
        {
            var gotStart = _fromStart.TryGetIndexInDirection(
                key: section.Min,
                higher: false,
                result: out var start);
            var gotEndStart = _fromStart.TryGetIndexInDirection(
                key: section.Max,
                higher: true,
                result: out var endStart);
            var gotEnd = _fromEnd.TryGetIndexInDirection(
                key: section.Max,
                higher: true,
                result: out var end);
            if (!gotStart || !gotEnd || !gotEndStart)
            {
                records = default!;
                return false;
            }

            var endLocation = _fromEnd.Keys[end];
            var endStartLocation = _fromStart.Keys[endStart];
            if (endLocation > endStartLocation)
            {
                records = default!;
                return false;
            }

            var ret = new HashSet<RecordLocationMarker>();
            for (int i = start; i <= end; i++)
            {
                ret.Add(_fromStart.Values[i]);
            }

            records = ret;
            return true;
        }

        public IEnumerable<long> GetContainingGroupLocations(FormKey formKey)
        {
            return _fromFormKeys[formKey].GroupPositions;
        }
    }
}