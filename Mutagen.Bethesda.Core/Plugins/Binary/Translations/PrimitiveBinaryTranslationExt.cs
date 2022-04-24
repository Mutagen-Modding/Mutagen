using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;

namespace Mutagen.Bethesda.Translations.Binary;

public static class PrimitiveBinaryTranslationExt
{
    public static void Write<TItem, TReader>(
        this PrimitiveBinaryTranslation<TItem, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        TItem item,
        RecordType header,
        Action<MutagenWriter, TItem>? write = null)
        where TItem : struct
        where TReader : IMutagenReadStream
    {
        write ??= transl.Write;
        try
        {
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                write(writer, item);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }

    public static void WriteNullable<TItem, TReader>(
        this PrimitiveBinaryTranslation<TItem, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        TItem? item,
        RecordType header,
        Action<MutagenWriter, TItem>? write = null)
        where TItem : struct
        where TReader : IMutagenReadStream
    {
        if (!item.HasValue) return;
        write ??= transl.Write;
        try
        {
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                write(writer, item.Value);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }

    public static void WriteNullable<TItem, TReader>(
        this PrimitiveBinaryTranslation<TItem, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        TItem? item,
        Action<MutagenWriter, TItem>? write = null)
        where TItem : struct
        where TReader : IMutagenReadStream
    {
        if (!item.HasValue) return;
        write ??= transl.Write;
        write(writer, item.Value);
    }
}
