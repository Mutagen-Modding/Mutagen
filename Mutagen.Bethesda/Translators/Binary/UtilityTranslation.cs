using Loqui;
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
        public static M MajorRecordParse<M, E>(
            M record,
            MutagenFrame frame,
            Func<E> errorMask,
            RecordType recType,
            RecordTypeConverter recordTypeConverter,
            Action<M, MutagenFrame, Func<E>> fillStructs,
            Func<M, MutagenFrame, Func<E>, RecordTypeConverter, TryGet<int?>> fillTyped)
            where M : MajorRecord
            where E : IErrorMask
        {
            try
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
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask().Overall = ex;
            }
            return record;
        }
    }
}
