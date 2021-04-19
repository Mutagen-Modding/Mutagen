using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Constants;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class RecordLocator
    {
        #region Get File Locations
        internal class FileLocationConstructor
        {
            public Dictionary<FormKey, (RangeInt64 Range, IEnumerable<long> GroupPositions, RecordType Record)> FromFormKeys = new Dictionary<FormKey, (RangeInt64 Range, IEnumerable<long> GroupPositions, RecordType Record)>();
            public List<long> FromStartPositions = new List<long>();
            public List<long> FromEndPositions = new List<long>();
            public List<(FormKey FormKey, RecordType Record)> FormKeys = new List<(FormKey FormKey, RecordType Record)>();
            public List<long> GrupLocations = new List<long>();
            public FormKey LastParsed;
            public long LastLoc;
            public GameConstants MetaData { get; }
            public Func<IMutagenReadStream, RecordType, uint, bool>? AdditionalCriteria;

            public FileLocationConstructor(GameConstants metaData)
            {
                this.MetaData = metaData;
            }

            public void Add(
                FormKey formKey,
                RecordType record,
                Stack<long> parentGrupLocations,
                RangeInt64 section)
            {
                var grupArr = parentGrupLocations.ToArray();
                this.FromFormKeys[formKey] = (section, grupArr, record);
                this.FromStartPositions.Add(section.Min);
                this.FromEndPositions.Add(section.Max);
                this.FormKeys.Add((formKey, record));
                this.LastParsed = formKey;
                this.LastLoc = section.Min;
            }
        }

        public class FileLocations
        {
            private readonly Dictionary<FormKey, (RangeInt64 Range, IEnumerable<long> GroupPositions, RecordType Record)> _fromFormKeys;
            private readonly SortingListDictionary<long, (FormKey FormKey, RecordType Record)> _fromStart;
            private readonly SortingListDictionary<long, (FormKey FormKey, RecordType Record)> _fromEnd;
            private readonly SortingListDictionary<long, long> _grupLocations;
            public ISortedListGetter<long> GrupLocations => _grupLocations.Keys;
            public SortingListDictionary<long, (FormKey FormKey, RecordType Record)> ListedRecords => _fromStart;
            public RangeInt64 this[FormKey formKey] => _fromFormKeys[formKey].Range;
            public ICollectionGetter<FormKey> FormKeys => new CollectionGetterWrapper<FormKey>(_fromFormKeys.Keys);

            internal FileLocations(FileLocationConstructor constructor)
            {
                _fromFormKeys = constructor.FromFormKeys;
                _fromStart = SortingListDictionary<long, (FormKey FormKey, RecordType Record)>.Factory_Wrap_AssumeSorted(
                    constructor.FromStartPositions,
                    constructor.FormKeys);
                _fromEnd = SortingListDictionary<long, (FormKey FormKey, RecordType Record)>.Factory_Wrap_AssumeSorted(
                    constructor.FromEndPositions,
                    constructor.FormKeys);
                var set = new HashSet<long>();
                _grupLocations = new SortingListDictionary<long, long>(constructor.GrupLocations.Select(i => new KeyValuePair<long, long>(i, i)));
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

            public bool TryGetRecord(long loc, out (FormKey FormKey, RecordType Record) record)
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

            public bool TryGetRecords(RangeInt64 section, [MaybeNullWhen(false)] out IEnumerable<(FormKey FormKey, RecordType Record)> records)
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
                var ret = new HashSet<(FormKey FormKey, RecordType Record)>();
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

        public static FileLocations GetFileLocations(
            ModPath filePath,
            GameConstants constants,
            RecordInterest? interest = null)
        {
            using var stream = new MutagenBinaryReadStream(
                filePath,
                new ParsingBundle(
                    constants, 
                    MasterReferenceReader.FromPath(filePath, constants.Release)));
            return GetFileLocations(stream, interest);
        }

        private static void SkipHeader(IMutagenReadStream reader)
        {
            if (!reader.TryReadModHeader(out var header))
            {
                reader.Position = reader.Length;
                return;
            }
            reader.Position += header.ContentLength;
        }

        public static FileLocations GetFileLocations(
            IMutagenReadStream reader,
            RecordInterest? interest = null,
            Func<IMutagenReadStream, RecordType, uint, bool>? additionalCriteria = null)
        {
            FileLocationConstructor ret = new FileLocationConstructor(reader.MetaData.Constants)
            {
                AdditionalCriteria = additionalCriteria,
            };
            SkipHeader(reader);

            HashSet<RecordType>? remainingTypes = ((interest?.InterestingTypes?.Count ?? 0) <= 0) ? null : new HashSet<RecordType>(interest!.InterestingTypes!);
            Stack<long> grupPositions = new Stack<long>();
            while (!reader.Complete
                && (remainingTypes?.Count ?? 1) > 0)
            {
                var parsed = ParseTopLevelGRUP(
                    reader: reader,
                    fileLocs: ret,
                    interest: interest,
                    grupRecOverride: null,
                    checkOverallGrupType: true,
                    parentGroupLocations: grupPositions);
                if (parsed.HasValue)
                {
                    remainingTypes?.Remove(parsed.Value);
                }
            }
            return new FileLocations(ret);
        }

        private static RecordType? ParseTopLevelGRUP(
            IMutagenReadStream reader,
            FileLocationConstructor fileLocs,
            RecordInterest? interest,
            Stack<long> parentGroupLocations,
            RecordType? grupRecOverride,
            bool checkOverallGrupType)
        {
            var grupLoc = reader.Position;
            GroupHeader groupMeta = reader.GetGroup();
            fileLocs.GrupLocations.Add(reader.Position);
            var grupRec = grupRecOverride ?? groupMeta.ContainedRecordType;

            if (checkOverallGrupType
                && (!interest?.IsInterested(grupRec) ?? false))
            { // Skip
                reader.Position += groupMeta.TotalLength;
                return null;
            }

            reader.Position += groupMeta.HeaderLength;

            using (var frame = MutagenFrame.ByFinalPosition(reader, reader.Position + groupMeta.ContentLength))
            {
                while (!frame.Complete)
                {
                    MajorRecordHeader majorRecordMeta = frame.GetMajorRecord();
                    var targetRec = majorRecordMeta.RecordType;
                    if (targetRec != grupRec)
                    {
                        var grup = frame.GetGroup(checkIsGroup: false);
                        if (IsSubLevelGRUP(grup))
                        {
                            parentGroupLocations.Push(grupLoc);
                            HandleSubLevelGRUP(
                                frame: frame,
                                fileLocs: fileLocs,
                                parentGroupLocations: parentGroupLocations,
                                interest: interest);
                            parentGroupLocations.Pop();
                            continue;
                        }
                        else if (checkOverallGrupType)
                        {
                            throw new ArgumentException($"Target Record {targetRec} at {frame.Position} did not match its containing GRUP: {grupRec}");
                        }
                    }
                    var recLength = majorRecordMeta.ContentLength;
                    if (fileLocs.AdditionalCriteria != null)
                    {
                        var pos = reader.Position;
                        if (!fileLocs.AdditionalCriteria(reader, targetRec, recLength))
                        {
                            reader.Position = pos + majorRecordMeta.TotalLength;
                            continue;
                        }
                        reader.Position = pos;
                    }
                    if (interest?.IsInterested(targetRec) ?? true)
                    {
                        parentGroupLocations.Push(grupLoc);
                        var pos = reader.Position;
                        var currentFormKey = FormKey.Factory(reader.MetaData.MasterReferences!, majorRecordMeta.FormID.Raw);

                        fileLocs.Add(
                            formKey: currentFormKey,
                            record: targetRec,
                            parentGrupLocations: parentGroupLocations,
                            section: new RangeInt64(pos, pos + majorRecordMeta.TotalLength - 1));
                        parentGroupLocations.Pop();
                    }
                    reader.Position += majorRecordMeta.TotalLength;
                }
            }

            return grupRec;
        }

        private static bool IsSubLevelGRUP(GroupHeader groupMeta)
        {
            if (!groupMeta.IsGroup)
            {
                return false;
            }
            return groupMeta.Meta.GroupConstants.HasSubGroups.Contains(groupMeta.GroupType);
        }

        private static void HandleSubLevelGRUP(
            MutagenFrame frame,
            FileLocationConstructor fileLocs,
            RecordInterest? interest,
            Stack<long> parentGroupLocations)
        {
            var grupLoc = frame.Position;
            GroupHeader groupMeta = frame.GetGroup();
            if (!groupMeta.IsGroup)
            {
                throw new DataMisalignedException("Group was not read in where expected: 0x" + (frame.Position - 4).ToString("X"));
            }
            fileLocs.GrupLocations.Add(grupLoc);
            var grupType = groupMeta.GroupType;
            if (grupType == frame.MetaData.Constants.GroupConstants.Cell.TopGroupType)
            {
                parentGroupLocations.Push(grupLoc);
                HandleCellSubchildren(
                    frame: frame.SpawnWithLength(groupMeta.TotalLength),
                    fileLocs: fileLocs,
                    parentGroupLocations: parentGroupLocations,
                    interest: interest);
                parentGroupLocations.Pop();
            }
            else if (grupType == frame.MetaData.Constants.GroupConstants.World.TopGroupType
                || grupType == frame.MetaData.Constants.GroupConstants.Topic?.TopGroupType
                || grupType == frame.MetaData.Constants.GroupConstants.Quest?.TopGroupType)
            {
                ParseTopLevelGRUP(
                    reader: frame.Reader,
                    fileLocs: fileLocs,
                    interest: interest,
                    parentGroupLocations: parentGroupLocations,
                    checkOverallGrupType: false,
                    grupRecOverride: null);
            }
            else if (frame.MetaData.Constants.GroupConstants.World.CellGroupTypes.Contains(grupType))
            {
                parentGroupLocations.Push(grupLoc);
                HandleCells(
                    frame: frame.SpawnWithLength(groupMeta.TotalLength),
                    fileLocs: fileLocs,
                    parentGroupLocations: parentGroupLocations,
                    interest: interest);
                parentGroupLocations.Pop();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void HandleCells(
            MutagenFrame frame,
            FileLocationConstructor fileLocs,
            RecordInterest? interest,
            Stack<long> parentGroupLocations)
        {
            frame.Reader.Position += fileLocs.MetaData.GroupConstants.HeaderLength;
            while (!frame.Complete)
            {
                var grupLoc = frame.Position;
                var groupMeta = frame.GetGroup();
                if (!groupMeta.IsGroup)
                {
                    throw new ArgumentException();
                }
                fileLocs.GrupLocations.Add(grupLoc);
                if (frame.MetaData.Constants.GroupConstants.World.CellSubGroupTypes.Contains(groupMeta.GroupType))
                {
                    ParseTopLevelGRUP(
                        reader: frame.Reader,
                        fileLocs: fileLocs,
                        interest: interest,
                        parentGroupLocations: parentGroupLocations,
                        grupRecOverride: new RecordType("CELL"),
                        checkOverallGrupType: true);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        private static void HandleCellSubchildren(
            MutagenFrame frame,
            FileLocationConstructor fileLocs,
            RecordInterest? interest,
            Stack<long> parentGroupLocations)
        {
            frame.Reader.Position += fileLocs.MetaData.GroupConstants.HeaderLength;
            while (!frame.Complete)
            {
                var grupLoc = frame.Position;
                var groupMeta = frame.GetGroup();
                fileLocs.GrupLocations.Add(grupLoc);
                if (frame.MetaData.Constants.GroupConstants.Cell.SubTypes.Contains(groupMeta.GroupType))
                {
                    ParseTopLevelGRUP(
                        reader: frame.Reader,
                        fileLocs: fileLocs,
                        interest: interest,
                        parentGroupLocations: parentGroupLocations,
                        grupRecOverride: null,
                        checkOverallGrupType: false);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
        #endregion

        #region Base GRUP Iterator
        public static IEnumerable<KeyValuePair<RecordType, long>> IterateBaseGroupLocations(
            IMutagenReadStream reader)
        {
            SkipHeader(reader);
            while (!reader.Complete)
            {
                GroupHeader groupMeta = reader.GetGroup();
                if (!groupMeta.IsGroup)
                {
                    throw new DataMisalignedException("Group was not read in where expected: 0x" + reader.Position.ToString("X"));
                }
                var pos = reader.Position + groupMeta.TotalLength;
                yield return new KeyValuePair<RecordType, long>(groupMeta.ContainedRecordType, reader.Position);
                reader.Position = pos;
            }
        }

        public static IEnumerable<(FormKey FormKey, long Position)> ParseTopLevelGRUP(
            IMutagenReadStream reader,
            bool checkOverallGrupType = true)
        {
            var groupMeta = reader.GetGroup();
            var grupLoc = reader.Position;
            var targetRec = groupMeta.ContainedRecordType;
            if (!groupMeta.IsGroup)
            {
                throw new ArgumentException();
            }

            reader.Position += groupMeta.HeaderLength;

            using (var frame = MutagenFrame.ByFinalPosition(reader, reader.Position + groupMeta.ContentLength))
            {
                while (!frame.Complete)
                {
                    var recordLocation = reader.Position;
                    MajorRecordHeader majorMeta = reader.GetMajorRecord();
                    if (majorMeta.RecordType != targetRec)
                    {
                        var subGroupMeta = reader.GetGroup();
                        if (IsSubLevelGRUP(subGroupMeta))
                        {
                            reader.Position += subGroupMeta.TotalLength;
                            continue;
                        }
                        else if (checkOverallGrupType)
                        {
                            throw new ArgumentException($"Target Record {targetRec} at {frame.Position} did not match its containing GRUP: {subGroupMeta.ContainedRecordType}");
                        }
                    }

                    var formKey = FormKey.Factory(reader.MetaData.MasterReferences!, majorMeta.FormID.Raw);
                    var len = majorMeta.TotalLength;
                    yield return (
                        formKey,
                        recordLocation);
                    reader.Position += len;
                }
            }
        }
        #endregion
    }
}
