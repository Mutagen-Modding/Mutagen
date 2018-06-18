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
                    using (var writer = new System.IO.BinaryWriter(new FileStream(outputPath.Path, FileMode.Create, FileAccess.Write)))
                    {
                        long runningDiff = 0;
                        var fileLocs = MajorRecordLocator.GetFileLocations(
                            inputStream,
                            interest,
                            additionalCriteria: (stream, recType, len) =>
                            {
                                stream.Position += 8;
                                var flags = (MajorRecord.MajorRecordFlag)inputStream.ReadInt32();
                                return flags.HasFlag(MajorRecord.MajorRecordFlag.Compressed);
                            });

                        // Construct group length container for later use
                        Dictionary<long, long> grupLengths = new Dictionary<long, long>();
                        Dictionary<long, long> grupOffsets = new Dictionary<long, long>();

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
                            writer.Write(inputStream.ReadBytes((int)noRecordLength));

                            // If complete overall, return
                            if (inputStream.Complete) return;

                            // Get compression status
                            var recLengthLocation = writer.BaseStream.Position;
                            var len = inputStream.ReadUInt32();
                            writer.Write(len);
                            var flags = (MajorRecord.MajorRecordFlag)inputStream.ReadInt32();
                            // Turn compressed flag off
                            flags &= ~MajorRecord.MajorRecordFlag.Compressed;
                            writer.Write((int)flags);

                            writer.Write(inputStream.ReadBytes(8));
                            using (var frame = MutagenFrame.ByLength(
                                reader: inputStream,
                                length: len,
                                snapToFinalPosition: false))
                            {
                                // Decompress
                                var decompressed = frame.Decompress();
                                var decompressedLen = decompressed.TotalLength;
                                writer.Write(decompressed.ReadRemaining());

                                // If no difference in lengths, move on
                                var lengthDiff = decompressedLen - len;
                                if (lengthDiff == 0) continue;

                                // Modify record length
                                writer.BaseStream.Position = recLengthLocation;
                                writer.Write((uint)(len + lengthDiff));

                                // Modify parent group lengths
                                foreach (var grupLoc in fileLocs.GetContainingGroupLocations(nextRec.Value))
                                {
                                    if (!grupOffsets.TryGetValue(grupLoc, out var offset))
                                    {
                                        offset = runningDiff;
                                        grupOffsets[grupLoc] = offset;
                                    }
                                    if (!grupLengths.TryGetValue(grupLoc, out var grupLen))
                                    {
                                        inputStreamJumpback.Position = grupLoc + 4;
                                        grupLen = inputStreamJumpback.ReadUInt32();
                                    }
                                    writer.BaseStream.Position = grupLoc + 4 + offset;
                                    writer.Write((uint)(grupLen + lengthDiff));
                                    grupLengths[grupLoc] = grupLen + lengthDiff;
                                }
                                runningDiff += lengthDiff;
                                writer.BaseStream.Position = writer.BaseStream.Length;
                            }
                        }
                    }
                }
            }
        }
    }
}
