using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
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
            this.FormKeys.Add(new(formKey, section, record));
            this.LastParsed = formKey;
            this.LastLoc = section.Min;
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
        var grupLoc = reader.Position;
        GroupHeader groupMeta = reader.GetGroupHeader();
        fileLocs.GrupLocations.Add(new GroupLocationMarker(RangeInt64.FromLength(reader.Position, groupMeta.TotalLength), groupMeta.ContainedRecordType));
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
        GroupHeader groupMeta = frame.GetGroupHeader();
        if (!groupMeta.IsGroup)
        {
            throw new DataMisalignedException("Group was not read in where expected: 0x" + (frame.Position - 4).ToString("X"));
        }
        fileLocs.GrupLocations.Add(new GroupLocationMarker(RangeInt64.FromLength(grupLoc, groupMeta.TotalLength), groupMeta.ContainedRecordType));
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
            var groupMeta = frame.GetGroupHeader();
            if (!groupMeta.IsGroup)
            {
                throw new ArgumentException();
            }
            fileLocs.GrupLocations.Add(new GroupLocationMarker(RangeInt64.FromLength(grupLoc, groupMeta.TotalLength), groupMeta.ContainedRecordType));
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
            var groupMeta = frame.GetGroupHeader();
            fileLocs.GrupLocations.Add(new GroupLocationMarker(RangeInt64.FromLength(grupLoc, groupMeta.TotalLength), groupMeta.ContainedRecordType));
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