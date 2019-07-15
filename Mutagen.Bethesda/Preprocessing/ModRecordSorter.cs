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

namespace Mutagen.Bethesda.Preprocessing
{
    public static class ModRecordSorter
    {
        public static void Sort(
            Func<Stream> streamCreator,
            Stream outputStream,
            GameMode gameMode)
        {
            var meta = MetaDataConstants.Get(gameMode);
            using (var inputStream = new MutagenBinaryReadStream(streamCreator(), meta))
            {
                using (var locatorStream = new MutagenBinaryReadStream(streamCreator(), meta))
                {
                    using (var writer = new MutagenWriter(outputStream, gameMode, dispose: false))
                    {
                        while (!inputStream.Complete)
                        {
                            long noRecordLength;
                            foreach (var grupLoc in RecordLocator.IterateBaseGroupLocations(locatorStream))
                            {
                                noRecordLength = grupLoc.Value - inputStream.Position;
                                inputStream.WriteTo(writer.BaseStream, (int)noRecordLength);

                                // If complete overall, return
                                if (inputStream.Complete) return;

                                var groupMeta = inputStream.MetaData.GetGroup(inputStream);
                                if (!groupMeta.IsGroup)
                                {
                                    throw new ArgumentException();
                                }

                                Dictionary<FormID, List<byte[]>> storage = new Dictionary<FormID, List<byte[]>>();
                                using (var grupFrame = new MutagenFrame(inputStream).SpawnWithLength(groupMeta.TotalLength))
                                {
                                    inputStream.WriteTo(writer.BaseStream, meta.GroupConstants.HeaderLength);
                                    locatorStream.Position = grupLoc.Value;
                                    foreach (var rec in RecordLocator.ParseTopLevelGRUP(locatorStream, gameMode))
                                    {
                                        MajorRecordMeta majorMeta = meta.GetMajorRecord(inputStream);
                                        storage.TryCreateValue(rec.FormID).Add(inputStream.ReadBytes(checked((int)majorMeta.TotalLength)));
                                        if (grupFrame.Complete) continue;
                                        GroupRecordMeta subGroupMeta = meta.GetGroup(inputStream);
                                        if (subGroupMeta.IsGroup)
                                        {
                                            storage.TryCreateValue(rec.FormID).Add(inputStream.ReadBytes(checked((int)subGroupMeta.TotalLength)));
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
        }
    }
}
