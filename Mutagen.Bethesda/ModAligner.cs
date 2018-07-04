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

namespace Mutagen.Bethesda
{
    public static class ModAligner
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

            public abstract byte[] GetBytes(BinaryReadStream inputStream);
        }

        public class AlignmentStraightRecord : AlignmentRule
        {
            private RecordType _recordType;

            public AlignmentStraightRecord(string str)
            {
                _recordType = new RecordType(str);
            }

            public override RecordType RecordType => _recordType;

            public override byte[] GetBytes(BinaryReadStream inputStream)
            {
                var subType = HeaderTranslation.ReadNextSubRecordType(
                    inputStream,
                    out var subLen);
                if (!subType.Equals(_recordType))
                {
                    throw new ArgumentException();
                }
                var ret = new byte[subLen + 6];
                MutagenWriter stream = new MutagenWriter(new MemoryStream(ret));
                using (HeaderExport.ExportSubRecordHeader(stream, _recordType))
                {
                    inputStream.WriteTo(stream.Writer.BaseStream, subLen);
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

            public override byte[] GetBytes(BinaryReadStream inputStream)
            {
                Dictionary<RecordType, byte[]> dataDict = new Dictionary<RecordType, byte[]>();
                MutagenWriter stream;
                while (!inputStream.Complete)
                {
                    var subType = HeaderTranslation.ReadNextSubRecordType(
                        inputStream,
                        out var subLen);
                    if (!SubTypes.Contains(subType))
                    {
                        inputStream.Position -= 6;
                        break;
                    }
                    var data = new byte[subLen + 6];
                    stream = new MutagenWriter(new MemoryStream(data));
                    using (HeaderExport.ExportSubRecordHeader(stream, subType))
                    {
                        inputStream.WriteTo(stream.Writer.BaseStream, subLen);
                    }
                    dataDict[subType] = data;
                }
                byte[] ret = new byte[dataDict.Values.Sum((d) => d.Length)];
                stream = new MutagenWriter(new MemoryStream(ret));
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
            AlignmentRules alignmentRules,
            TempFolder temp = null)
        {
            var interest = new RecordInterest(alignmentRules.Alignments.Keys);
            var fileLocs = MajorRecordLocator.GetFileLocations(inputPath.Path, interest);
            temp = temp ?? new TempFolder();
            using (temp)
            {
                var alignedMajorRecordsFile = Path.Combine(temp.Dir.Path, "alignedRules");
                using (var inputStream = new BinaryReadStream(inputPath.Path))
                {
                    using (var writer = new MutagenWriter(new FileStream(alignedMajorRecordsFile, FileMode.Create)))
                    {
                        AlignMajorRecordsByRules(inputStream, writer, alignmentRules, fileLocs);
                    }
                }

                var alignedGroupsFile = Path.Combine(temp.Dir.Path, "alignedGroups");
                using (var inputStream = new BinaryReadStream(alignedMajorRecordsFile))
                {
                    using (var writer = new MutagenWriter(new FileStream(alignedGroupsFile, FileMode.Create)))
                    {
                        AlignGroupsByRules(inputStream, writer, alignmentRules, fileLocs);
                    }
                }

                fileLocs = MajorRecordLocator.GetFileLocations(alignedGroupsFile, interest);
                var alignedCellsFile = Path.Combine(temp.Dir.Path, "alignedCells");
                using (var mutaReader = new BinaryReadStream(alignedGroupsFile))
                {
                    using (var writer = new MutagenWriter(alignedCellsFile))
                    {
                        foreach (var grup in fileLocs.GrupLocations)
                        {
                            if (grup <= mutaReader.Position) continue;
                            var noRecordLength = grup - mutaReader.Position;
                            mutaReader.WriteTo(writer.Writer.BaseStream, (int)noRecordLength);

                            // If complete overall, return
                            if (mutaReader.Complete) break;

                            mutaReader.WriteTo(writer.Writer.BaseStream, 12);
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
                        mutaReader.WriteTo(writer.Writer.BaseStream, checked((int)mutaReader.Remaining));
                    }
                }

                fileLocs = MajorRecordLocator.GetFileLocations(alignedCellsFile, interest);
                using (var mutaReader = new BinaryReadStream(alignedCellsFile))
                {
                    using (var writer = new MutagenWriter(outputPath.Path))
                    {
                        foreach (var grup in fileLocs.GrupLocations)
                        {
                            if (grup <= mutaReader.Position) continue;
                            var noRecordLength = grup - mutaReader.Position;
                            mutaReader.WriteTo(writer.Writer.BaseStream, (int)noRecordLength);

                            // If complete overall, return
                            if (mutaReader.Complete) break;

                            mutaReader.WriteTo(writer.Writer.BaseStream, 12);
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
                        mutaReader.WriteTo(writer.Writer.BaseStream, checked((int)mutaReader.Remaining));
                    }
                }
            }
        }

        private static void AlignMajorRecordsByRules(
            BinaryReadStream inputStream,
            MutagenWriter writer,
            AlignmentRules alignmentRules,
            MajorRecordLocator.FileLocations fileLocs)
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
                inputStream.WriteTo(writer.Writer.BaseStream, (int)noRecordLength);

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
                inputStream.WriteTo(writer.Writer.BaseStream, 12);
                var endPos = inputStream.Position + len;
                Dictionary<RecordType, byte[]> dataDict = new Dictionary<RecordType, byte[]>();
                byte[] rest = null;
                while (inputStream.Position < endPos)
                {
                    var subType = HeaderTranslation.GetNextSubRecordType(
                        inputStream,
                        out var subLen);
                    if (stopMarkers?.Contains(subType) ?? false)
                    {
                        rest = inputStream.ReadBytes((int)(endPos - inputStream.Position));
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
                    writer.Write(rest);
                }
            }
        }

        private static void AlignGroupsByRules(
            BinaryReadStream inputStream,
            MutagenWriter writer,
            AlignmentRules alignmentRules,
            MajorRecordLocator.FileLocations fileLocs)
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
                inputStream.WriteTo(writer.Writer.BaseStream, (int)noRecordLength);

                // If complete overall, return
                if (inputStream.Complete) break;
                var recType = HeaderTranslation.ReadNextRecordType(
                    inputStream,
                    out var len);
                writer.Write(recType.TypeInt);
                writer.Write(len);
                if (!recType.Equals(Group_Registration.GRUP_HEADER))
                {
                    throw new ArgumentException();
                }
                inputStream.WriteTo(writer.Writer.BaseStream, 4);
                var groupType = (GroupTypeEnum)inputStream.ReadInt32();
                writer.Write((int)groupType);

                if (!alignmentRules.GroupAlignment.TryGetValue(groupType, out var groupRules)) continue;

                inputStream.WriteTo(writer.Writer.BaseStream, 4);
                Dictionary<RecordType, List<byte[]>> storage = new Dictionary<RecordType, List<byte[]>>();
                List<byte[]> rest = new List<byte[]>();
                using (var frame = MutagenFrame.ByLength(inputStream, len - 20))
                {
                    while (!frame.Complete)
                    {
                        var type = HeaderTranslation.GetNextSubRecordType(inputStream, out var recLength);
                        var bytes = inputStream.ReadBytes(recLength + Constants.RECORD_HEADER_LENGTH);
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
            writer.Write(mutaReader.ReadBytes(4));
            Dictionary<GroupTypeEnum, byte[]> storage = new Dictionary<GroupTypeEnum, byte[]>();
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
                storage[subGrupType] = mutaReader.ReadBytes(subLen);
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
            BinaryReadStream mutaReader,
            MutagenWriter writer)
        {
            mutaReader.WriteTo(writer.Writer.BaseStream, 4);
            byte[] roadStorage = null;
            byte[] cellStorage = null;
            List<byte[]> grupBytes = new List<byte[]>();
            for (int i = 0; i < 3; i++)
            {
                RecordType type = HeaderTranslation.ReadNextRecordType(mutaReader);
                switch (type.Type)
                {
                    case "ROAD":
                        var roadLen = mutaReader.ReadUInt32();
                        mutaReader.Position -= 8;
                        roadStorage = mutaReader.ReadBytes((int)(Constants.RECORD_HEADER_LENGTH + roadLen));
                        break;
                    case "CELL":
                        if (cellStorage != null)
                        {
                            throw new ArgumentException();
                        }
                        var startPos = mutaReader.Position - 4;
                        var cellLen = mutaReader.ReadUInt32();
                        mutaReader.Position += Constants.RECORD_HEADER_LENGTH - 8 + cellLen;
                        var grupPos = mutaReader.Position;
                        var grup = HeaderTranslation.ReadNextRecordType(mutaReader);
                        uint cellGrupLen;
                        if (grup == Group_Registration.GRUP_HEADER)
                        {
                            cellGrupLen = mutaReader.ReadUInt32();
                            mutaReader.Position += 4;
                            var grupType = (GroupTypeEnum)mutaReader.ReadInt32();
                            if (grupType != GroupTypeEnum.CellChildren)
                            {
                                cellGrupLen = 0;
                            }
                        }
                        else
                        {
                            cellGrupLen = 0;
                        }
                        mutaReader.Position = startPos;
                        cellStorage = mutaReader.ReadBytes((int)(Constants.RECORD_HEADER_LENGTH + cellLen + cellGrupLen));
                        break;
                    case "GRUP":
                        if (roadStorage != null
                            && cellStorage != null)
                        {
                            i = 3; // end loop
                            mutaReader.Position -= 4;
                            continue;
                        }
                        var grupLen = mutaReader.ReadUInt32();
                        mutaReader.Position -= 8;
                        grupBytes.Add(mutaReader.ReadBytes((int)grupLen));
                        break;
                    case "WRLD":
                        mutaReader.Position -= 4;
                        i = 3; // end loop
                        continue;
                    default:
                        throw new NotImplementedException();
                }
            }
            if (roadStorage != null)
            {
                writer.Write(roadStorage);
            }
            if (cellStorage != null)
            {
                writer.Write(cellStorage);
            }
            foreach (var item in grupBytes)
            {
                writer.Write(item);
            }
        }
    }
}
