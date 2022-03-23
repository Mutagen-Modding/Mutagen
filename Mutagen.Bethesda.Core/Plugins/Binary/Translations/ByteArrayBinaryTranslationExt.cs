using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public static class ByteArrayBinaryTranslationExt
{
    public static void Write<TReader>(
        this ByteArrayBinaryTranslation<TReader, MutagenWriter> transl,
        MutagenWriter writer,
        ReadOnlyMemorySlice<byte>? item,
        RecordType header)
        where TReader : IMutagenReadStream
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
            throw SubrecordException.Enrich(ex, header);
        }
    }

    public static void Write<TReader>(
        this ByteArrayBinaryTranslation<TReader, MutagenWriter> transl,
        MutagenWriter writer,
        ReadOnlyMemorySlice<byte>? item,
        RecordType header,
        RecordType overflowRecord)
        where TReader : IMutagenReadStream
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
            throw SubrecordException.Enrich(ex, header);
        }
    }
}
