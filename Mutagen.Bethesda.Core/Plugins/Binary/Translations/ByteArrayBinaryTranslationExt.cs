using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public static class ByteArrayBinaryTranslationExt
{
    public static void Write<TReader>(
        this ByteArrayBinaryTranslation<TReader, MutagenWriter> transl,
        MutagenWriter writer,
        ReadOnlyMemorySlice<byte>? item,
        RecordType header,
        RecordType? overflowRecord = null,
        RecordType? markerType = null)
        where TReader : IMutagenReadStream
    {
        try
        {
            if (!item.HasValue) return;
            if (overflowRecord.HasValue && item.Value.Length > ushort.MaxValue)
            {
                using (HeaderExport.Subrecord(writer, overflowRecord.Value))
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
                if (markerType.HasValue)
                {
                    using (HeaderExport.Subrecord(writer, markerType.Value))
                    {
                    }
                }
                using (HeaderExport.Subrecord(writer, header))
                {
                    transl.Write(writer, item.Value.Span);
                }
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }
}
