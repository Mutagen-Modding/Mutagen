using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;
using System;
using Mutagen.Bethesda.Plugins.Meta;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public static class BooleanBinaryTranslationExt
{
    public static void WriteAsMarker<TReader>(
        this PrimitiveBinaryTranslation<bool, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        bool item,
        RecordType header)
        where TReader : IMutagenReadStream
    {
        try
        {
            if (!item) return;
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                // Presence of marker signifies true
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Factory(ex, header);
        }
    }

    public static void Write<TReader>(
        this PrimitiveBinaryTranslation<bool, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        bool item,
        RecordType header,
        byte byteLength)
        where TReader : IMutagenReadStream
    {
        try
        {
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                writer.Write(item ? 1 : 0, byteLength);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Factory(ex, header);
        }
    }

    public static void WriteNullable<TReader>(
        this PrimitiveBinaryTranslation<bool, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        bool? item,
        RecordType header,
        byte byteLength)
        where TReader : IMutagenReadStream
    {
        try
        {
            if (!item.HasValue) return;
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                writer.Write(item.Value ? 1 : 0, byteLength);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Factory(ex, header);
        }
    }
}
