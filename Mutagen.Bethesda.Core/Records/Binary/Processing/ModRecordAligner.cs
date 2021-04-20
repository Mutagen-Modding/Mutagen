using Mutagen.Bethesda.Constants;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Records.Binary.Headers;
using Mutagen.Bethesda.Records.Binary.Streams;
using Mutagen.Bethesda.Records.Binary.Translations;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mutagen.Bethesda.Records.Binary.Processing
{
    public static class ModRecordAligner
    {
        public class AlignmentRules
        {
            public Dictionary<RecordType, Dictionary<RecordType, AlignmentRule>> Alignments = new Dictionary<RecordType, Dictionary<RecordType, AlignmentRule>>();
            public Dictionary<RecordType, IEnumerable<RecordType>> StopMarkers = new Dictionary<RecordType, IEnumerable<RecordType>>();
            public Dictionary<int, List<RecordType>> GroupTypeAlignment = new Dictionary<int, List<RecordType>>();
            public GameRelease GameRelease;

            public AlignmentRules(GameRelease gameMode)
            {
                this.GameRelease = gameMode;
            }

            public void AddAlignments(RecordType type, params RecordType[] recTypes)
            {
                var subList = new Dictionary<RecordType, AlignmentRule>();
                foreach (var t in recTypes)
                {
                    subList[t] = new AlignmentStraightRecord(t.Type, this.GameRelease);
                }
                this.Alignments.Add(
                    type,
                    subList);
            }

            public void AddAlignments(RecordType type, params AlignmentRule[] rules)
            {
                var dict = Alignments.GetOrAdd(type);
                foreach (var rule in rules)
                {
                    dict[rule.RecordType] = rule;
                }
            }

            public void SetGroupAlignment(int group, params RecordType[] recTypes)
            {
                GroupTypeAlignment.GetOrAdd(group).SetTo(recTypes);
            }
        }

        public abstract class AlignmentRule
        {
            public abstract RecordType RecordType { get; }

            public abstract byte[] GetBytes(IMutagenReadStream inputStream);
        }

        public class AlignmentStraightRecord : AlignmentRule
        {
            private RecordType _recordType;
            private GameRelease _gameMode;

            public AlignmentStraightRecord(string str, GameRelease gameMode)
            {
                _recordType = new RecordType(str);
                _gameMode = gameMode;
            }

            public override RecordType RecordType => _recordType;

            public override byte[] GetBytes(IMutagenReadStream inputStream)
            {
                var subType = HeaderTranslation.ReadNextSubrecordType(
                    inputStream,
                    out var subLen);
                if (!subType.Equals(_recordType))
                {
                    throw new ArgumentException();
                }
                var ret = new byte[subLen + 6];
                MutagenWriter stream = new MutagenWriter(new MemoryStream(ret), this._gameMode);
                using (HeaderExport.Subrecord(stream, _recordType))
                {
                    inputStream.WriteTo(stream.BaseStream, subLen);
                }
                return ret;
            }
        }

        public class AlignmentSubRule : AlignmentRule
        {
            public List<RecordType> SubTypes = new List<RecordType>();
            public GameRelease GameRelease;

            public AlignmentSubRule(
                GameRelease release,
                params RecordType[] types)
            {
                this.SubTypes = types.ToList();
                this.GameRelease = release;
            }

            public override RecordType RecordType => SubTypes[0];

            public override byte[] GetBytes(IMutagenReadStream inputStream)
            {
                Dictionary<RecordType, byte[]> dataDict = new Dictionary<RecordType, byte[]>();
                MutagenWriter stream;
                while (!inputStream.Complete)
                {
                    var subType = HeaderTranslation.ReadNextSubrecordType(
                        inputStream,
                        out var subLen);
                    if (!SubTypes.Contains(subType))
                    {
                        inputStream.Position -= 6;
                        break;
                    }
                    var data = new byte[subLen + 6];
                    stream = new MutagenWriter(new MemoryStream(data), this.GameRelease);
                    using (HeaderExport.Subrecord(stream, subType))
                    {
                        inputStream.WriteTo(stream.BaseStream, subLen);
                    }
                    dataDict[subType] = data;
                }
                byte[] ret = new byte[dataDict.Values.Sum((d) => d.Length)];
                stream = new MutagenWriter(new MemoryStream(ret), this.GameRelease);
                foreach (var alignment in SubTypes)
                {
                    if (dataDict.TryGetValue(
                        alignment,
                        out var data))
                    {
                        stream.Write(data);
                    }
                }
                return ret;
            }
        }

        public static void Align(
            ModPath inputPath,
            FilePath outputPath,
            GameRelease release,
            AlignmentRules alignmentRules,
            TempFolder? temp = null)
        {
            var interest = new RecordInterest(alignmentRules.Alignments.Keys)
            {
                EmptyMeansInterested = false
            };
            var constants = GameConstants.Get(release);
            var parsingBundle = new ParsingBundle(constants, MasterReferenceReader.FromPath(inputPath, release));
            var fileLocs = RecordLocator.GetFileLocations(inputPath.Path, release, interest);
            temp ??= TempFolder.Factory();
            using (temp)
            {
                var alignedMajorRecordsFile = Path.Combine(temp.Dir.Path, "alignedRules");
                using (var inputStream = new MutagenBinaryReadStream(inputPath.Path, parsingBundle))
                {
                    using var writer = new MutagenWriter(new FileStream(alignedMajorRecordsFile, FileMode.Create), release);
                    AlignMajorRecordsByRules(inputStream, writer, alignmentRules, fileLocs);
                }

                var alignedGroupsFile = Path.Combine(temp.Dir.Path, "alignedGroups");
                using (var inputStream = new MutagenBinaryReadStream(alignedMajorRecordsFile, parsingBundle))
                {
                    using var writer = new MutagenWriter(new FileStream(alignedGroupsFile, FileMode.Create), release);
                    AlignGroupsByRules(inputStream, writer, alignmentRules, fileLocs);
                }

                fileLocs = RecordLocator.GetFileLocations(alignedGroupsFile, release, interest);
                var alignedCellsFile = Path.Combine(temp.Dir.Path, "alignedCells");
                using (var mutaReader = new BinaryReadStream(alignedGroupsFile))
                {
                    using var writer = new MutagenWriter(alignedCellsFile, release);
                    foreach (var grup in fileLocs.GrupLocations)
                    {
                        if (grup <= mutaReader.Position) continue;
                        var noRecordLength = grup - mutaReader.Position;
                        mutaReader.WriteTo(writer.BaseStream, (int)noRecordLength);

                        // If complete overall, return
                        if (mutaReader.Complete) break;

                        mutaReader.WriteTo(writer.BaseStream, 12);
                        var grupType = mutaReader.ReadUInt32();
                        writer.Write((int)grupType);
                        if (grupType == constants.GroupConstants.Cell.TopGroupType)
                        {
                            AlignCellChildren(mutaReader, writer);
                        }
                    }
                    mutaReader.WriteTo(writer.BaseStream, checked((int)mutaReader.Remaining));
                }

                fileLocs = RecordLocator.GetFileLocations(alignedCellsFile, release, interest);
                using (var mutaReader = new MutagenBinaryReadStream(alignedCellsFile, parsingBundle))
                {
                    using var writer = new MutagenWriter(outputPath.Path, GameConstants.Get(release));
                    foreach (var grup in fileLocs.GrupLocations)
                    {
                        if (grup <= mutaReader.Position) continue;
                        var noRecordLength = grup - mutaReader.Position;
                        mutaReader.WriteTo(writer.BaseStream, (int)noRecordLength);

                        // If complete overall, return
                        if (mutaReader.Complete) break;

                        mutaReader.WriteTo(writer.BaseStream, 12);
                        var grupType = mutaReader.ReadUInt32();
                        writer.Write((int)grupType);
                        if (grupType == constants.GroupConstants.World.TopGroupType)
                        {
                            AlignWorldChildren(mutaReader, writer);
                        }
                    }
                    mutaReader.WriteTo(writer.BaseStream, checked((int)mutaReader.Remaining));
                }
            }
        }

        private static void AlignMajorRecordsByRules(
            IMutagenReadStream inputStream,
            MutagenWriter writer,
            AlignmentRules alignmentRules,
            RecordLocator.FileLocations fileLocs)
        {
            while (!inputStream.Complete)
            {
                // Import until next listed major record
                long noRecordLength;
                if (fileLocs.ListedRecords.TryGetInDirection(
                    inputStream.Position,
                    higher: true,
                    result: out var nextRec))
                {
                    var recordLocation = fileLocs.ListedRecords.Keys[nextRec.Key];
                    noRecordLength = recordLocation - inputStream.Position;
                }
                else
                {
                    noRecordLength = inputStream.Remaining;
                }
                inputStream.WriteTo(writer.BaseStream, (int)noRecordLength);

                // If complete overall, return
                if (inputStream.Complete) break;

                var recType = HeaderTranslation.ReadNextRecordType(
                    inputStream,
                    out var len);
                IEnumerable<RecordType>? stopMarkers;
                if (!alignmentRules.StopMarkers.TryGetValue(recType, out stopMarkers))
                {
                    stopMarkers = null;
                }
                if (!alignmentRules.Alignments.TryGetValue(recType, out var alignments))
                {
                    throw new ArgumentException($"Encountered an unknown record: {recType}");
                }
                writer.Write(recType.TypeInt);
                writer.Write(len);
                inputStream.WriteTo(writer.BaseStream, 12);
                var endPos = inputStream.Position + len;
                Dictionary<RecordType, byte[]> dataDict = new Dictionary<RecordType, byte[]>();
                ReadOnlyMemorySlice<byte>? rest = null;
                while (inputStream.Position < endPos)
                {
                    var subType = HeaderTranslation.GetNextSubrecordType(
                        inputStream,
                        out var subLen);
                    if (stopMarkers?.Contains(subType) ?? false)
                    {
                        rest = inputStream.ReadMemory((int)(endPos - inputStream.Position), readSafe: true);
                        break;
                    }
                    if (!alignments.TryGetValue(subType, out var rule))
                    {
                        throw new ArgumentException($"Encountered an unknown record: {subType}");
                    }
                    dataDict[subType] = rule.GetBytes(inputStream);
                }
                foreach (var alignment in alignmentRules.Alignments[recType])
                {
                    if (dataDict.TryGetValue(
                        alignment.Key,
                        out var data))
                    {
                        writer.Write(data);
                        dataDict.Remove(alignment.Key);
                    }
                }
                if (dataDict.Count > 0)
                {
                    throw new ArgumentException($"Encountered an unknown record: {dataDict.First().Key}");
                }
                if (rest != null)
                {
                    writer.Write(rest.Value);
                }
            }
        }

        private static void AlignGroupsByRules(
            MutagenBinaryReadStream inputStream,
            MutagenWriter writer,
            AlignmentRules alignmentRules,
            RecordLocator.FileLocations fileLocs)
        {
            while (!inputStream.Complete)
            {
                // Import until next listed major record
                long noRecordLength;
                if (fileLocs.GrupLocations.TryGetInDirection(
                    inputStream.Position,
                    higher: true,
                    result: out var nextRec))
                {
                    noRecordLength = nextRec.Value - inputStream.Position;
                }
                else
                {
                    noRecordLength = inputStream.Remaining;
                }
                inputStream.WriteTo(writer.BaseStream, (int)noRecordLength);

                // If complete overall, return
                if (inputStream.Complete) break;
                var groupMeta = inputStream.GetGroup();
                if (!groupMeta.IsGroup)
                {
                    throw new ArgumentException();
                }
                inputStream.WriteTo(writer.BaseStream, checked((int)groupMeta.HeaderLength));

                if (!alignmentRules.GroupTypeAlignment.TryGetValue(groupMeta.GroupType, out var groupRules)) continue;
                
                var storage = new Dictionary<RecordType, List<ReadOnlyMemorySlice<byte>>>();
                var rest = new List<ReadOnlyMemorySlice<byte>>();
                using (var frame = MutagenFrame.ByLength(inputStream, groupMeta.ContentLength))
                {
                    while (!frame.Complete)
                    {
                        var majorMeta = inputStream.GetMajorRecord();
                        var bytes = inputStream.ReadMemory(checked((int)majorMeta.TotalLength), readSafe: true);
                        var type = majorMeta.RecordType;
                        if (groupRules.Contains(type))
                        {
                            storage.GetOrAdd(type).Add(bytes);
                        }
                        else
                        {
                            rest.Add(bytes);
                        }
                    }
                }
                foreach (var rule in groupRules)
                {
                    if (storage.TryGetValue(rule, out var storageBytes))
                    {
                        foreach (var item in storageBytes)
                        {
                            writer.Write(item);
                        }
                    }
                }
                foreach (var item in rest)
                {
                    writer.Write(item);
                }
            }
        }

        private static void AlignCellChildren(
            BinaryReadStream mutaReader,
            MutagenWriter writer)
        {
            writer.Write(mutaReader.ReadSpan(4, readSafe: false));
            var storage = new Dictionary<int, ReadOnlyMemorySlice<byte>>();
            for (int i = 0; i < 3; i++)
            {
                mutaReader.Position += 4;
                var subLen = mutaReader.ReadInt32();
                mutaReader.Position += 4;
                var subGrupType = mutaReader.ReadInt32();
                mutaReader.Position -= 16;
                if (!writer.MetaData.Constants.GroupConstants.Cell.SubTypes.Contains(subGrupType))
                {
                    i = 3; // end loop
                    continue;
                }
                storage[subGrupType] = mutaReader.ReadMemory(subLen, readSafe: true);
            }
            foreach (var item in writer.MetaData.Constants.GroupConstants.Cell.SubTypes)
            {
                if (storage.TryGetValue(item, out var content))
                {
                    writer.Write(content);
                }
            }
        }

        private static void AlignWorldChildren(
            IMutagenReadStream reader,
            MutagenWriter writer)
        {
            reader.WriteTo(writer.BaseStream, 4);
            ReadOnlyMemorySlice<byte>? roadStorage = null;
            ReadOnlyMemorySlice<byte>? cellStorage = null;
            var grupBytes = new List<ReadOnlyMemorySlice<byte>>();
            for (int i = 0; i < 3; i++)
            {
                MajorRecordHeader majorMeta = reader.GetMajorRecord();
                switch (majorMeta.RecordType.Type)
                {
                    case "ROAD":
                        roadStorage = reader.ReadMemory(checked((int)majorMeta.TotalLength));
                        break;
                    case "CELL":
                        if (cellStorage != null)
                        {
                            throw new ArgumentException();
                        }
                        var startPos = reader.Position;
                        var cellMeta = reader.GetMajorRecord();
                        reader.Position += cellMeta.TotalLength;
                        var cellGroupMeta = reader.GetGroup();
                        long cellGrupLen;
                        if (cellGroupMeta.IsGroup
                            && cellGroupMeta.GroupType == writer.MetaData.Constants.GroupConstants.Cell.TopGroupType)
                        {
                            cellGrupLen = cellGroupMeta.TotalLength;
                        }
                        else
                        {
                            cellGrupLen = 0;
                        }
                        reader.Position = startPos;
                        cellStorage = reader.ReadMemory(checked((int)(cellMeta.TotalLength + cellGrupLen)));
                        break;
                    case "GRUP":
                        if (roadStorage != null
                            && cellStorage != null)
                        {
                            i = 3; // end loop
                            continue;
                        }
                        var groupMeta = reader.GetGroup();
                        grupBytes.Add(reader.ReadMemory(checked((int)groupMeta.TotalLength)));
                        break;
                    case "WRLD":
                        i = 3; // end loop
                        continue;
                    default:
                        throw new NotImplementedException();
                }
            }
            if (roadStorage != null)
            {
                writer.Write(roadStorage.Value);
            }
            if (cellStorage != null)
            {
                writer.Write(cellStorage.Value);
            }
            foreach (var item in grupBytes)
            {
                writer.Write(item);
            }
        }
    }
}
