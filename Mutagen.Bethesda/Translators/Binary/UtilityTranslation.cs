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
        public static M MajorRecordParse<M>(
            M record,
            MutagenFrame frame,
            ErrorMaskBuilder errorMask,
            RecordType recType,
            RecordTypeConverter recordTypeConverter,
            MasterReferences masterReferences,
            Action<M, MutagenFrame, MasterReferences, ErrorMaskBuilder> fillStructs,
            Func<M, MutagenFrame, MasterReferences, ErrorMaskBuilder, RecordTypeConverter, TryGet<int?>> fillTyped)
            where M : MajorRecord
        {
            frame = frame.SpawnWithFinalPosition(HeaderTranslation.ParseRecord(
                frame.Reader,
                recType));
            fillStructs(
                record,
                frame,
                masterReferences,
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
                                masterReferences,
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
                            masterReferences,
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
