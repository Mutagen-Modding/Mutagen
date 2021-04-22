using Mutagen.Bethesda.Records.Binary.Streams;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public static class ByteArrayBinaryTranslationExt
    {
        public static void Write(
            this ByteArrayBinaryTranslation<MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            ReadOnlyMemorySlice<byte>? item,
            RecordType header)
        {
            try
            {
                if (!item.HasValue) return;
                using (HeaderExport.Subrecord(writer, header))
                {
                    transl.Write(writer, item.Value.Span);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }

        public static void Write(
            this ByteArrayBinaryTranslation<MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            ReadOnlyMemorySlice<byte>? item,
            RecordType header,
            RecordType overflowRecord)
        {
            try
            {
                if (!item.HasValue) return;
                if (item.Value.Length > ushort.MaxValue)
                {
                    using (HeaderExport.Subrecord(writer, overflowRecord))
                    {
                        writer.Write(item.Value.Length);
                    }
                    using (HeaderExport.Subrecord(writer, header))
                    {
                    }
                    transl.Write(writer, item.Value.Span);
                }
                else
                {
                    using (HeaderExport.Subrecord(writer, header))
                    {
                        transl.Write(writer, item.Value.Span);
                    }
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }
    }
}
