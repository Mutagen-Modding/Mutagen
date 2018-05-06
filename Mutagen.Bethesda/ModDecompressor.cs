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
            Stream inputStream,
            Stream outputStream,
            RecordInterest interest = null)
        {
            long runningDiff = 0;
            var fileLocs = MajorRecordLocator.GetFileLocations(inputStream, interest);
            using (var mutaReader = new MutagenReader(inputStream))
            {
                // Construct group length container for later use
                Dictionary<long, long> grupLengths = new Dictionary<long, long>();
                foreach (var grupLoc in fileLocs.GrupLocations)
                {
                    mutaReader.Position = grupLoc + 4;
                    grupLengths[grupLoc] = mutaReader.ReadUInt32();
                }

                Dictionary<long, long> grupOffsets = new Dictionary<long, long>();

                mutaReader.Position = 0;
                using (var writer = new System.IO.BinaryWriter(outputStream))
                {
                    while (!mutaReader.Complete)
                    {
                        // Import until next listed major record
                        long noRecordLength;
                        if (fileLocs.ListedRecords.TryGetInDirection(
                            mutaReader.Position,
                            higher: true,
                            result: out var nextRec))
                        {
                            var recordLocation = fileLocs.ListedRecords.Keys[nextRec.Key];
                            noRecordLength = recordLocation - mutaReader.Position;
                        }
                        else
                        {
                            noRecordLength = mutaReader.Length - mutaReader.Position;
                        }
                        noRecordLength += 4;
                        writer.Write(mutaReader.ReadBytes((int)noRecordLength));
                        
                        // If complete overall, return
                        if (mutaReader.Complete) return;

                        // Get compression status
                        var recLengthLocation = writer.BaseStream.Position;
                        var len = mutaReader.ReadUInt32();
                        writer.Write(len);
                        var flags = (MajorRecord.MajorRecordFlag)mutaReader.ReadInt32();

                        if (!flags.HasFlag(MajorRecord.MajorRecordFlag.Compressed))
                        {
                            writer.Write((int)flags);
                            continue;
                        }
                        
                        // Turn compressed flag off
                        flags &= ~MajorRecord.MajorRecordFlag.Compressed;
                        writer.Write((int)flags);

                        writer.Write(mutaReader.ReadBytes(8));
                        using (var frame = MutagenFrame.ByLength(
                            reader: mutaReader,
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
                                var grupLen = grupLengths[grupLoc];
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

        public static void Decompress(
            FilePath inputPath,
            FilePath outputPath,
            RecordInterest interest = null)
        {
            using (var inputStream = new FileStream(inputPath.Path, FileMode.Open, FileAccess.Read))
            {
                using (var outputStream = new FileStream(outputPath.Path, FileMode.Create, FileAccess.Write))
                {
                    Decompress(inputStream, outputStream, interest);
                }
            }
        }
    }
}
