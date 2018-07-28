using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class ModDecompressor
    {
        public static void Decompress(
            FilePath inputPath,
            FilePath outputPath,
            RecordInterest interest = null)
        {
            using (var inputStream = new BinaryReadStream(inputPath.Path))
            {
                using (var inputStreamJumpback = new BinaryReadStream(inputPath.Path))
                {
                    var fs = new FileStream(outputPath.Path, FileMode.Create, FileAccess.Write);
                    using (var writer = new System.IO.BinaryWriter(fs))
                    {
                        long runningDiff = 0;
                        var fileLocs = RecordLocator.GetFileLocations(
                            inputStream,
                            interest,
                            additionalCriteria: (stream, recType, len) =>
                            {
                                stream.Position += 8;
                                var flags = (MajorRecord.MajorRecordFlag)inputStream.ReadInt32();
                                return flags.HasFlag(MajorRecord.MajorRecordFlag.Compressed);
                            });

                        // Construct group length container for later use
                        Dictionary<long, (uint Length, long Offset)> grupMeta = new Dictionary<long, (uint Length, long Offset)>();

                        inputStream.Position = 0;
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
                                noRecordLength += 4;
                            }
                            else
                            {
                                noRecordLength = inputStream.Length - inputStream.Position;
                            }
                            inputStream.WriteTo(fs, (int)noRecordLength);

                            // If complete overall, return
                            if (inputStream.Complete) break;

                            // Get compression status
                            var len = inputStream.ReadUInt32();
                            var flags = (MajorRecord.MajorRecordFlag)inputStream.ReadInt32();

                            // Turn compressed flag off
                            flags &= ~MajorRecord.MajorRecordFlag.Compressed;
                            var restOfMeta = inputStream.ReadBytes(8);
                            using (var frame = MutagenFrame.ByLength(
                                reader: inputStream,
                                length: len,
                                snapToFinalPosition: false))
                            {
                                // Decompress
                                var decompressed = frame.Decompress();
                                var decompressedLen = decompressed.TotalLength;
                                var lengthDiff = decompressedLen - len;
                                writer.Write((uint)(len + lengthDiff));
                                writer.Write((int)flags);
                                writer.Write(restOfMeta);
                                writer.Write(decompressed.ReadRemaining());

                                // If no difference in lengths, move on
                                if (lengthDiff == 0) continue;

                                // Modify parent group lengths
                                foreach (var grupLoc in fileLocs.GetContainingGroupLocations(nextRec.Value.FormID))
                                {
                                    if (!grupMeta.TryGetValue(grupLoc, out var meta))
                                    {
                                        meta.Offset = runningDiff;
                                        inputStreamJumpback.Position = grupLoc + 4;
                                        meta.Length = inputStreamJumpback.ReadUInt32();
                                    }
                                    grupMeta[grupLoc] = ((uint)(meta.Length + lengthDiff), meta.Offset);
                                }
                                runningDiff += lengthDiff;
                            }
                        }

                        foreach (var item in grupMeta)
                        {
                            var grupLoc = item.Key;
                            fs.Position = grupLoc + 4 + item.Value.Offset;
                            writer.Write(item.Value.Length);
                        }
                    }
                }
            }
        }
    }
}
