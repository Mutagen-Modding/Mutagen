using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Plugins.Utility;

namespace Mutagen.Bethesda.Plugins.Analysis;

public static class RecordLocator
{
    #region Get File Locations
    internal class FileLocationConstructor
    {
        public Dictionary<FormKey, (RangeInt64 Range, IEnumerable<long> GroupPositions, RecordType Record)> FromFormKeys = new();
        public List<long> FromStartPositions = new();
        public List<long> FromEndPositions = new();
        public List<RecordLocationMarker> FormKeys = new();
        public List<GroupLocationMarker> GrupLocations = new();
        public FormKey LastParsed;
        public long LastLoc;
        public GameConstants MetaData { get; }
        public Func<IMutagenReadStream, RecordType, uint, bool>? AdditionalCriteria;

        public FileLocationConstructor(GameConstants metaData)
        {
            MetaData = metaData;
        }

        public void Add(
            FormKey formKey,
            RecordType record,
            Stack<long> parentGrupLocations,
            RangeInt64 section)
        {
            var grupArr = parentGrupLocations.ToArray();
            FromFormKeys[formKey] = (section, grupArr, record);
            FromStartPositions.Add(section.Min);
            FromEndPositions.Add(section.Max);
            FormKeys.Add(new(formKey, section, record));
            LastParsed = formKey;
            LastLoc = section.Min;
        }
    }

    public static RecordLocatorResults GetLocations(
        ModPath filePath,
        GameConstants constants,
        RecordInterest? interest = null)
    {
        using var stream = new MutagenBinaryReadStream(
            filePath,
            new ParsingBundle(
                constants, 
                MasterReferenceCollection.FromPath(filePath, constants.Release)));
        return GetLocations(stream, interest);
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

    public static RecordLocatorResults GetLocations(
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
        return new RecordLocatorResults(ret);
    }

    private static RecordType? ParseTopLevelGRUP(
        IMutagenReadStream reader,
        FileLocationConstructor fileLocs,
        RecordInterest? interest,
        Stack<long> parentGroupLocations,
        RecordType? grupRecOverride,
        bool checkOverallGrupType)
    {
        var groupMeta = reader.GetGroupHeader();
        fileLocs.GrupLocations.Add(new GroupLocationMarker(groupMeta));
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
                MajorRecordHeader majorRecordMeta = frame.GetMajorRecordHeader();
                var targetRec = majorRecordMeta.RecordType;
                if (targetRec != grupRec)
                {
                    var grup = frame.GetGroupHeader(checkIsGroup: false);
                    if (IsSubLevelGRUP(grup))
                    {
                        parentGroupLocations.Push(groupMeta.Location);
                        HandleSubLevelGRUP(
                            frame: frame,
                            fileLocs: fileLocs,
                            parentGrup: grup,
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
                    parentGroupLocations.Push(groupMeta.Location);
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
        GroupHeader parentGrup,
        FileLocationConstructor fileLocs,
        RecordInterest? interest,
        Stack<long> parentGroupLocations)
    {
        var groupMeta = frame.GetGroupHeader();
        fileLocs.GrupLocations.Add(new GroupLocationMarker(groupMeta));
        var grupType = groupMeta.GroupType;
        if (grupType == frame.MetaData.Constants.GroupConstants.World.TopGroupType
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
            return;
        }
        else if (frame.MetaData.Constants.GroupConstants.World.CellGroupTypes.Contains(grupType))
        {
            parentGroupLocations.Push(groupMeta.Location);
            HandleCells(
                frame: frame.SpawnWithLength(groupMeta.TotalLength),
                fileLocs: fileLocs,
                parentGroupLocations: parentGroupLocations,
                interest: interest);
            parentGroupLocations.Pop();
            return;
        }

        var nesting = frame.MetaData.Constants.GroupConstants.Nesting.FirstOrDefault(r => r.GroupType == parentGrup.GroupType);
        if (nesting != null)
        {
            HandleNesting(
                frame: frame.SpawnWithLength(groupMeta.TotalLength),
                fileLocs: fileLocs,
                parentGroupLocations: parentGroupLocations,
                nesting: nesting,
                parentGroupLoc: groupMeta.Location,
                interest: interest);
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
            var groupMeta = frame.GetGroupHeader();
            if (!groupMeta.IsGroup)
            {
                throw new ArgumentException();
            }
            fileLocs.GrupLocations.Add(new GroupLocationMarker(groupMeta));
            if (frame.MetaData.Constants.GroupConstants.World.CellSubGroupTypes.Contains(groupMeta.GroupType))
            {
                ParseTopLevelGRUP(
                    reader: frame.Reader,
                    fileLocs: fileLocs,
                    interest: interest,
                    parentGroupLocations: parentGroupLocations,
                    grupRecOverride: RecordTypes.CELL,
                    checkOverallGrupType: true);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    private static void HandleNesting(
        MutagenFrame frame,
        FileLocationConstructor fileLocs,
        RecordInterest? interest,
        long parentGroupLoc,
        GroupNesting nesting,
        Stack<long> parentGroupLocations)
    {
        if (nesting.Underneath.Length == 0)
        {
            ParseTopLevelGRUP(
                reader: frame.Reader,
                fileLocs: fileLocs,
                interest: interest,
                parentGroupLocations: parentGroupLocations,
                grupRecOverride: nesting.ContainedRecordType,
                checkOverallGrupType: nesting.ContainedRecordType != null);
        }
        else
        {
            parentGroupLocations.Push(parentGroupLoc);
            frame.Reader.Position += fileLocs.MetaData.GroupConstants.HeaderLength;
            while (!frame.Complete)
            {
                var groupMeta = frame.GetGroupHeader();
                fileLocs.GrupLocations.Add(new GroupLocationMarker(groupMeta));
                var targetNesting = nesting.Underneath.FirstOrDefault(x => x.GroupType == groupMeta.GroupType);
                if (targetNesting == null)
                {
                    throw new NotImplementedException();
                }
                HandleNesting(frame, fileLocs, interest, groupMeta.Location, targetNesting, parentGroupLocations);
            }
            parentGroupLocations.Pop();
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
            GroupHeader groupMeta = reader.GetGroupHeader();
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
        var groupMeta = reader.GetGroupHeader();
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
                MajorRecordHeader majorMeta = reader.GetMajorRecordHeader();
                if (majorMeta.RecordType != targetRec)
                {
                    var subGroupMeta = reader.GetGroupHeader();
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