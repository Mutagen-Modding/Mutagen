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
            using (var inputStream = new MutagenBinaryReadStream(streamCreator()))
            {
                using (var locatorStream = new MutagenBinaryReadStream(streamCreator()))
                {
                    using (var writer = new MutagenWriter(outputStream, gameMode, dispose: false))
                    {
                        while (!inputStream.Complete)
                        {
                            long noRecordLength;
                            foreach (var grupLoc in RecordLocator.IterateBaseGroupLocations(locatorStream, gameMode))
                            {
                                noRecordLength = grupLoc.Value - inputStream.Position;
                                inputStream.WriteTo(writer.BaseStream, (int)noRecordLength);

                                // If complete overall, return
                                if (inputStream.Complete) return;

                                HeaderTranslation.GetNextRecordType(inputStream, out var grupLen);

                                Dictionary<FormID, List<byte[]>> storage = new Dictionary<FormID, List<byte[]>>();
                                using (var grupFrame = new MutagenFrame(inputStream).SpawnWithLength(grupLen))
                                {
                                    inputStream.WriteTo(writer.BaseStream, Constants.GRUP_LENGTH);
                                    locatorStream.Position = grupLoc.Value;
                                    foreach (var rec in RecordLocator.ParseTopLevelGRUP(locatorStream))
                                    {
                                        var type = HeaderTranslation.GetNextRecordType(inputStream, out var len);
                                        storage.TryCreateValue(rec.FormID).Add(inputStream.ReadBytes(len + Constants.RECORD_HEADER_LENGTH));
                                        if (grupFrame.Complete) continue;
                                        var nextType = HeaderTranslation.GetNextRecordType(inputStream, out var subGrupLen);
                                        if (nextType.Equals(Group_Registration.GRUP_HEADER))
                                        {
                                            storage.TryCreateValue(rec.FormID).Add(inputStream.ReadBytes(subGrupLen));
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
