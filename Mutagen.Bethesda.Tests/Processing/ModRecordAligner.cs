using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public static class ModRecordAligner
    {
        public class AlignmentRules
        {
            public Dictionary<RecordType, Dictionary<RecordType, AlignmentRule>> Alignments = new Dictionary<RecordType, Dictionary<RecordType, AlignmentRule>>();
            public Dictionary<RecordType, IEnumerable<RecordType>> StopMarkers = new Dictionary<RecordType, IEnumerable<RecordType>>();
            public Dictionary<GroupTypeEnum, List<RecordType>> GroupAlignment = new Dictionary<GroupTypeEnum, List<RecordType>>();

            public void AddAlignments(RecordType type, params RecordType[] recTypes)
            {
                var subList = new Dictionary<RecordType, AlignmentRule>();
                foreach (var t in recTypes)
                {
                    subList[t] = new AlignmentStraightRecord(t.Type);
                }
                this.Alignments.Add(
                    type,
                    subList);
            }

            public void AddAlignments(RecordType type, params AlignmentRule[] rules)
            {
                var dict = Alignments.TryCreateValue(type);
                foreach (var rule in rules)
                {
                    dict[rule.RecordType] = rule;
                }
            }

            public void SetGroupAlignment(GroupTypeEnum group, params RecordType[] recTypes)
            {
                GroupAlignment.TryCreateValue(group).SetTo(recTypes);
            }
        }

        public abstract class AlignmentRule
        {
            public abstract RecordType RecordType { get; }

            public abstract ReadOnlyMemorySlice<byte> GetBytes(IMutagenReadStream inputStream);
        }

        public class AlignmentStraightRecord : AlignmentRule
        {
            private RecordType _recordType;

            public AlignmentStraightRecord(string str)
            {
                _recordType = new RecordType(str);
            }

            public override RecordType RecordType => _recordType;

            public override ReadOnlyMemorySlice<byte> GetBytes(IMutagenReadStream inputStream)
            {
                var subType = HeaderTranslation.ReadNextSubrecordType(
                    inputStream,
                    out var subLen);
                if (!subType.Equals(_recordType))
                {
                    throw new ArgumentException();
                }
                var ret = new byte[subLen + 6];
                MutagenWriter stream = new MutagenWriter(new MemoryStream(ret), inputStream.MetaData);
                using (HeaderExport.ExportSubrecordHeader(stream, _recordType))
                {
                    inputStream.WriteTo(stream.BaseStream, subLen);
                }
                return ret;
            }
        }

        public class AlignmentSubRule : AlignmentRule
        {
            public List<RecordType> SubTypes = new List<RecordType>();

            public AlignmentSubRule(
                params RecordType[] types)
            {
                this.SubTypes = types.ToList();
            }

            public override RecordType RecordType => SubTypes[0];

            public override ReadOnlyMemorySlice<byte> GetBytes(IMutagenReadStream inputStream)
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
                    stream = new MutagenWriter(new MemoryStream(data), inputStream.MetaData);
                    using (HeaderExport.ExportSubrecordHeader(stream, subType))
                    {
                        inputStream.WriteTo(stream.BaseStream, subLen);
                    }
                    dataDict[subType] = data;
                }
                byte[] ret = new byte[dataDict.Values.Sum((d) => d.Length)];
                stream = new MutagenWriter(new MemoryStream(ret), inputStream.MetaData);
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
            FilePath inputPath,
            FilePath outputPath,
            GameMode gameMode,
            AlignmentRules alignmentRules,
            TempFolder temp = null)
        {
            var interest = new Mutagen.Bethesda.RecordInterest(alignmentRules.Alignments.Keys)
            {
                EmptyMeansInterested = false
            };
            var fileLocs = RecordLocator.GetFileLocations(inputPath.Path, gameMode, interest);
            temp = temp ?? new TempFolder();
            using (temp)
            {
                var alignedMajorRecordsFile = Path.Combine(temp.Dir.Path, "alignedRules");
                using (var inputStream = new MutagenBinaryReadStream(inputPath.Path, gameMode))
                {
                    using (var writer = new MutagenWriter(new FileStream(alignedMajorRecordsFile, FileMode.Create), gameMode))
                    {
                        AlignMajorRecordsByRules(inputStream, writer, alignmentRules, fileLocs);
                    }
                }

                var alignedGroupsFile = Path.Combine(temp.Dir.Path, "alignedGroups");
                using (var inputStream = new MutagenBinaryReadStream(alignedMajorRecordsFile, gameMode))
                {
                    using (var writer = new MutagenWriter(new FileStream(alignedGroupsFile, FileMode.Create), gameMode))
                    {
                        AlignGroupsByRules(inputStream, writer, alignmentRules, fileLocs);
                    }
                }

                fileLocs = RecordLocator.GetFileLocations(alignedGroupsFile, gameMode, interest);
                var alignedCellsFile = Path.Combine(temp.Dir.Path, "alignedCells");
                using (var mutaReader = new BinaryReadStream(alignedGroupsFile))
                {
                    using (var writer = new MutagenWriter(alignedCellsFile, gameMode))
                    {
                        foreach (var grup in fileLocs.GrupLocations)
                        {
                            if (grup <= mutaReader.Position) continue;
                            var noRecordLength = grup - mutaReader.Position;
                            mutaReader.WriteTo(writer.BaseStream, (int)noRecordLength);

                            // If complete overall, return
                            if (mutaReader.Complete) break;

                            mutaReader.WriteTo(writer.BaseStream, 12);
                            var grupType = (GroupTypeEnum)mutaReader.ReadUInt32();
                            writer.Write((int)grupType);
                            switch (grupType)
                            {
                                case GroupTypeEnum.CellChildren:
                                    AlignCellChildren(mutaReader, writer);
                                    break;
                                default:
                                    break;
                            }
                        }
                        mutaReader.WriteTo(writer.BaseStream, checked((int)mutaReader.Remaining));
                    }
                }

                fileLocs = RecordLocator.GetFileLocations(alignedCellsFile, gameMode, interest);
                using (var mutaReader = new MutagenBinaryReadStream(alignedCellsFile, gameMode))
                {
                    using (var writer = new MutagenWriter(outputPath.Path, gameMode))
                    {
                        foreach (var grup in fileLocs.GrupLocations)
                        {
                            if (grup <= mutaReader.Position) continue;
                            var noRecordLength = grup - mutaReader.Position;
                            mutaReader.WriteTo(writer.BaseStream, (int)noRecordLength);

                            // If complete overall, return
                            if (mutaReader.Complete) break;

                            mutaReader.WriteTo(writer.BaseStream, 12);
                            var grupType = (GroupTypeEnum)mutaReader.ReadUInt32();
                            writer.Write((int)grupType);
                            switch (grupType)
                            {
                                case GroupTypeEnum.WorldChildren:
                                    AlignWorldChildren(mutaReader, writer);
                                    break;
                                default:
                                    break;
                            }
                        }
                        mutaReader.WriteTo(writer.BaseStream, checked((int)mutaReader.Remaining));
                    }
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
                IEnumerable<RecordType> stopMarkers;
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
                var dataDict = new Dictionary<RecordType, ReadOnlyMemorySlice<byte>>();
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
                writer.Write(inputStream.ReadSpan(groupMeta.HeaderLength));

                if (!alignmentRules.GroupAlignment.TryGetValue((GroupTypeEnum)groupMeta.GroupType, out var groupRules)) continue;

                var storage = new Dictionary<RecordType, List<ReadOnlyMemorySlice<byte>>>();
                var rest = new List<ReadOnlyMemorySlice<byte>>();
                using (var frame = MutagenFrame.ByLength(inputStream, groupMeta.ContentLength))
                {
                    while (!frame.Complete)
                    {
                        var majorMeta = inputStream.GetMajorRecord();
                        var bytes = inputStream.ReadMemory(checked((int)majorMeta.TotalLength));
                        var type = majorMeta.RecordType;
                        if (groupRules.Contains(type))
                        {
                            storage.TryCreateValue(type).Add(bytes);
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
            var storage = new Dictionary<GroupTypeEnum, ReadOnlyMemorySlice<byte>>();
            for (int i = 0; i < 3; i++)
            {
                mutaReader.Position += 4;
                var subLen = mutaReader.ReadInt32();
                mutaReader.Position += 4;
                var subGrupType = (GroupTypeEnum)mutaReader.ReadUInt32();
                mutaReader.Position -= 16;
                switch (subGrupType)
                {
                    case GroupTypeEnum.CellPersistentChildren:
                    case GroupTypeEnum.CellTemporaryChildren:
                    case GroupTypeEnum.CellVisibleDistantChildren:
                        break;
                    default:
                        i = 3; // end loop
                        continue;
                }
                storage[subGrupType] = mutaReader.ReadMemory(subLen, readSafe: true);
            }
            if (storage.TryGetValue(GroupTypeEnum.CellPersistentChildren, out var content))
            {
                writer.Write(content);
            }
            if (storage.TryGetValue(GroupTypeEnum.CellTemporaryChildren, out content))
            {
                writer.Write(content);
            }
            if (storage.TryGetValue(GroupTypeEnum.CellVisibleDistantChildren, out content))
            {
                writer.Write(content);
            }
        }

        private static void AlignWorldChildren(
            IMutagenReadStream reader,
            MutagenWriter writer)
        {
            reader.WriteTo(writer.BaseStream, 4);
            ReadOnlyMemorySlice<byte>? roadStorage = null;
            ReadOnlyMemorySlice<byte>? cellStorage = null;
            List<ReadOnlyMemorySlice<byte>> grupBytes = new List<ReadOnlyMemorySlice<byte>>();
            for (int i = 0; i < 3; i++)
            {
                RecordType type = HeaderTranslation.GetNextRecordType(reader);
                switch (type.Type)
                {
                    case "ROAD":
                        roadStorage = reader.ReadMemory(checked((int)reader.GetMajorRecord().TotalLength));
                        break;
                    case "CELL":
                        if (cellStorage != null)
                        {
                            throw new ArgumentException();
                        }
                        var cellMajorMeta = reader.GetMajorRecord();
                        var startPos = reader.Position;
                        reader.Position += cellMajorMeta.HeaderLength;
                        var grupPos = reader.Position;
                        var cellSubGroupMeta = reader.GetGroup();
                        long cellGroupLen = 0;
                        if (cellSubGroupMeta.IsGroup
                            && cellSubGroupMeta.GroupType == (int)GroupTypeEnum.CellChildren)
                        {
                            cellGroupLen = cellSubGroupMeta.TotalLength;
                        }
                        reader.Position = startPos;
                        cellStorage = reader.ReadMemory(checked((int)(cellMajorMeta.TotalLength + cellGroupLen)));
                        break;
                    case "GRUP":
                        if (roadStorage != null
                            && cellStorage != null)
                        {
                            i = 3; // end loop
                            continue;
                        }
                        grupBytes.Add(reader.ReadMemory(checked((int)reader.GetGroup().TotalLength)));
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
