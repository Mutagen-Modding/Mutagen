using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using Mutagen.Bethesda.Plugins.Analysis;

namespace Mutagen.Bethesda.Plugins.Binary.Processing;

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
            inputStream.ReadModHeaderFrame();
            while (inputStream.Complete)
            {
                var grupLoc = inputStream.Position;
                noRecordLength = grupLoc - inputStream.Position;
                inputStream.WriteTo(writer.BaseStream, (int)noRecordLength);

                // If complete overall, return
                if (inputStream.Complete) return;

                var groupMeta = inputStream.GetGroupHeader();

                var storage = new Dictionary<FormKey, List<ReadOnlyMemorySlice<byte>>>();
                using (var grupFrame = new MutagenFrame(inputStream).SpawnWithLength(groupMeta.TotalLength))
                {
                    inputStream.WriteTo(writer.BaseStream, inputStream.MetaData.Constants.GroupConstants.HeaderLength);
                    locatorStream.Position = grupLoc;
                    foreach (var rec in RecordLocator.ParseTopLevelGRUP(locatorStream))
                    {
                        MajorRecordHeader majorMeta = inputStream.GetMajorRecordHeader();
                        storage.GetOrAdd(rec.FormKey).Add(inputStream.ReadMemory(checked((int)majorMeta.TotalLength), readSafe: true));
                        if (grupFrame.Complete) continue;
                        if (inputStream.TryGetGroupHeader(out var subGroupMeta))
                        {
                            storage.GetOrAdd(rec.FormKey).Add(inputStream.ReadMemory(checked((int)subGroupMeta.TotalLength), readSafe: true));
                        }
                    }
                }

                // Sorts via Record ID (as opposed to just the first 6 bytes)
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