using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class UtilityTranslation
    {
        public static M MajorRecordParse<M, I>(
            M record,
            MutagenFrame frame,
            ErrorMaskBuilder errorMask,
            RecordType recType,
            RecordTypeConverter recordTypeConverter,
            Action<M, MutagenFrame, ErrorMaskBuilder> fillStructs,
            Func<M, MutagenFrame, ErrorMaskBuilder, RecordTypeConverter, TryGet<I?>> fillTyped)
            where M : MajorRecord
            where I : struct
        {
            frame = frame.SpawnWithFinalPosition(HeaderTranslation.ParseRecord(
                frame.Reader,
                recType));
            fillStructs(
                record,
                frame,
                errorMask);
            if (fillTyped == null) return record;
            if (record.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed))
            {
                using (frame)
                {
                    var decompressed = frame.Decompress();
                    using (decompressed)
                    {
                        while (!decompressed.Complete)
                        {
                            var parsed = fillTyped(
                                record,
                                decompressed,
                                errorMask,
                                recordTypeConverter);
                            if (parsed.Failed) break;
                        }
                    }
                }
            }
            else
            {
                using (frame)
                {
                    while (!frame.Complete)
                    {
                        var parsed = fillTyped(
                            record,
                            frame,
                            errorMask,
                            recordTypeConverter);
                        if (parsed.Failed) break;
                    }
                }
            }
            return record;
        }
    }
}
