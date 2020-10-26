using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mutagen.Bethesda.Processing
{
    public static class ModRecordSorter
    {
        public static void Sort(
            Func<IMutagenReadStream> streamCreator,
            Stream outputStream,
            GameRelease release)
        {
            using var inputStream = streamCreator();
            using var locatorStream = streamCreator();
            using var writer = new MutagenWriter(outputStream, release, dispose: false);
            while (!inputStream.Complete)
            {
                long noRecordLength;
                foreach (var grupLoc in RecordLocator.IterateBaseGroupLocations(locatorStream))
                {
                    noRecordLength = grupLoc.Value - inputStream.Position;
                    inputStream.WriteTo(writer.BaseStream, (int)noRecordLength);

                    // If complete overall, return
                    if (inputStream.Complete) return;

                    var groupMeta = inputStream.GetGroup();

                    var storage = new Dictionary<FormID, List<ReadOnlyMemorySlice<byte>>>();
                    using (var grupFrame = new MutagenFrame(inputStream).SpawnWithLength(groupMeta.TotalLength))
                    {
                        inputStream.WriteTo(writer.BaseStream, inputStream.MetaData.Constants.GroupConstants.HeaderLength);
                        locatorStream.Position = grupLoc.Value;
                        foreach (var rec in RecordLocator.ParseTopLevelGRUP(locatorStream))
                        {
                            MajorRecordHeader majorMeta = inputStream.GetMajorRecord();
                            storage.GetOrAdd(rec.FormID).Add(inputStream.ReadMemory(checked((int)majorMeta.TotalLength), readSafe: true));
                            if (grupFrame.Complete) continue;
                            if (inputStream.TryGetGroup(out var subGroupMeta))
                            {
                                storage.GetOrAdd(rec.FormID).Add(inputStream.ReadMemory(checked((int)subGroupMeta.TotalLength), readSafe: true));
                            }
                        }
                    }

                    foreach (var item in storage.OrderBy((i) => i.Key.ID))
                    {
                        foreach (var bytes in item.Value)
                        {
                            writer.Write(bytes);
                        }
                    }
                }

                inputStream.WriteTo(writer.BaseStream, (int)inputStream.Remaining);
            }
        }
    }
}
