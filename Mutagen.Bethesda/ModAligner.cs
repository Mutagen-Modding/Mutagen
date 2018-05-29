using Mutagen.Bethesda.Binary;
using Noggog;
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
        }

        public static void Align(
            FilePath inputPath,
            FilePath outputPath,
            AlignmentRules alignmentRules)
        {
            using (var inputStream = new BinaryReadStream(inputPath.Path))
            {
                using (var outputStream = new FileStream(outputPath.Path, FileMode.Create, FileAccess.Write))
                {
                    Align(inputStream, outputStream, alignmentRules);
                }
            }
        }

        public static void Align(
            BinaryReadStream inputStream,
            Stream outputStream,
            AlignmentRules alignmentRules)
        {
            var interest = new RecordInterest(alignmentRules.Alignments.Keys);
            var fileLocs = MajorRecordLocator.GetFileLocations(inputStream, interest);
            var tempMemOutput = new MemoryStream();
            inputStream.Position = 0;
            using (var writer = new MutagenWriter(tempMemOutput, dispose: false))
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
                    writer.Write(inputStream.ReadBytes((int)noRecordLength));

                    // If complete overall, return
                    if (inputStream.Complete) break;

                    var recType = HeaderTranslation.ReadNextRecordType(
                        inputStream,
                        out var len);
                    writer.Write(recType.Type);
                    writer.Write(len);
                    writer.Write(inputStream.ReadBytes(12));

                    var endPos = inputStream.Position + len;
                    Dictionary<RecordType, byte[]> dataDict = new Dictionary<RecordType, byte[]>();
                    while (inputStream.Position < endPos)
                    {
                        var subType = HeaderTranslation.ReadNextSubRecordType(
                            inputStream,
                            out var subLen);
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
                }
            }

            tempMemOutput.Position = 0;
            using (var mutaReader = new BinaryMemoryStream(tempMemOutput.GetBuffer()))
            {
                using (var writer = new MutagenWriter(outputStream))
                {
                    foreach (var grup in fileLocs.GrupLocations)
                    {
                        if (grup <= mutaReader.Position) continue;
                        var noRecordLength = grup - mutaReader.Position;
                        writer.Write(mutaReader.ReadBytes(checked((int)noRecordLength)));

                        // If complete overall, return
                        if (mutaReader.Complete) break;

                        writer.Write(mutaReader.ReadBytes(12));
                        var grupType = (GroupTypeEnum)mutaReader.ReadUInt32();
                        writer.Write((int)grupType);
                        switch (grupType)
                        {
                            case GroupTypeEnum.CellChildren:
                                break;
                            default:
                                continue;
                        }
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
                    writer.Write(mutaReader.ReadBytes(checked((int)mutaReader.Remaining)));
                }
            }
        }
    }
}
