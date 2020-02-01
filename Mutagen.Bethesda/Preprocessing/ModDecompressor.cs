using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Preprocessing
{
    public static class ModDecompressor
    {
        public static void Decompress(
            Func<Stream> streamCreator,
            Stream outputStream,
            GameMode gameMode,
            RecordInterest? interest = null)
        {
            var meta = MetaDataConstants.Get(gameMode);
            using (var inputStream = new MutagenBinaryReadStream(streamCreator(), meta))
            {
                using (var inputStreamJumpback = new MutagenBinaryReadStream(streamCreator(), meta))
                {
                    using (var writer = new System.IO.BinaryWriter(outputStream, Encoding.Default, leaveOpen: true))
                    {
                        long runningDiff = 0;
                        var fileLocs = RecordLocator.GetFileLocations(
                            inputStream,
                            interest: interest,
                            additionalCriteria: (stream, recType, len) =>
                            {
                                return MetaDataConstants.Get(gameMode).GetMajorRecord(stream).IsCompressed;
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
                            }
                            else
                            {
                                noRecordLength = inputStream.Length - inputStream.Position;
                            }
                            inputStream.WriteTo(outputStream, (int)noRecordLength);

                            // If complete overall, return
                            if (inputStream.Complete) break;
                            var majorMeta = meta.ReadMajorRecord(inputStream);
                            var len = majorMeta.RecordLength;
                            using (var frame = MutagenFrame.ByLength(
                                reader: inputStream,
                                length: len))
                            {
                                // Decompress
                                var decompressed = frame.Decompress();
                                var decompressedLen = decompressed.TotalLength;
                                var lengthDiff = decompressedLen - len;
                                var majorMetaSpan = majorMeta.Span.ToArray();

                                // Write major Meta
                                var writableMajorMeta = meta.MajorRecordWritable(majorMetaSpan.AsSpan());
                                writableMajorMeta.IsCompressed = false;
                                writableMajorMeta.RecordLength = (uint)(len + lengthDiff);
                                writer.Write(majorMetaSpan);
                                writer.Write(decompressed.ReadRemaining());

                                // If no difference in lengths, move on
                                if (lengthDiff == 0) continue;

                                // Modify parent group lengths
                                foreach (var grupLoc in fileLocs.GetContainingGroupLocations(nextRec.Value.FormID))
                                {
                                    if (!grupMeta.TryGetValue(grupLoc, out var loc))
                                    {
                                        loc.Offset = runningDiff;
                                        inputStreamJumpback.Position = grupLoc + 4;
                                        loc.Length = inputStreamJumpback.ReadUInt32();
                                    }
                                    grupMeta[grupLoc] = ((uint)(loc.Length + lengthDiff), loc.Offset);
                                }
                                runningDiff += lengthDiff;
                            }
                        }

                        foreach (var item in grupMeta)
                        {
                            var grupLoc = item.Key;
                            outputStream.Position = grupLoc + 4 + item.Value.Offset;
                            writer.Write(item.Value.Length);
                        }
                    }
                }
            }
        }
    }
}
