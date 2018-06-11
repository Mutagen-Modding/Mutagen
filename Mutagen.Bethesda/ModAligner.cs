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
            public Dictionary<RecordType, IEnumerable<RecordType>> Alignments = new Dictionary<RecordType, IEnumerable<RecordType>>();
            public Dictionary<RecordType, IEnumerable<RecordType>> StopMarkers = new Dictionary<RecordType, IEnumerable<RecordType>>();
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
                var alignedRulesFile = Path.Combine(temp.Dir.Path, "alignedRules");
                using (var inputStream = new BinaryReadStream(inputPath.Path))
                {
                    using (var writer = new MutagenWriter(new FileStream(alignedRulesFile, FileMode.Create)))
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
                            writer.Write(recType.Type);
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
                                inputStream.Position += 6;
                                dataDict[subType] = inputStream.ReadBytes(subLen);
                            }
                            foreach (var alignment in alignmentRules.Alignments[recType])
                            {
                                if (dataDict.TryGetValue(
                                    alignment,
                                    out var data))
                                {
                                    using (HeaderExport.ExportSubRecordHeader(writer, alignment))
                                    {
                                        writer.Write(data);
                                    }
                                    dataDict.Remove(alignment);
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
                }

                fileLocs = MajorRecordLocator.GetFileLocations(alignedRulesFile, interest);
                var alignedCellsFile = Path.Combine(temp.Dir.Path, "alignedCells");
                using (var mutaReader = new BinaryReadStream(alignedRulesFile))
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
