using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Translations.Binary;
using System;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public static class EnumBinaryTranslationExt
{
    public static bool Parse<TEnum, TReader>(
        this EnumBinaryTranslation<TEnum, TReader, MutagenWriter> transl,
        TReader reader,
        out TEnum item)
        where TEnum : struct, Enum, IConvertible
        where TReader : IMutagenReadStream
    {
        return transl.Parse(reader, checked((int)reader.Remaining), out item);
    }

    public static TEnum Parse<TEnum, TReader>(
        this EnumBinaryTranslation<TEnum, TReader, MutagenWriter> transl,
        TReader reader)
        where TEnum : struct, Enum, IConvertible
        where TReader : IMutagenReadStream
    {
        return transl.Parse(reader, checked((int)reader.Remaining));
    }

    public static void Write<TEnum, TReader>(
        this EnumBinaryTranslation<TEnum, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        TEnum item,
        RecordType header,
        long length)
        where TEnum : struct, Enum, IConvertible
        where TReader : IMutagenReadStream
    {
        try
        {
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                transl.WriteValue(writer, item, length);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }

    public static void WriteNullable<TEnum, TReader>(
        this EnumBinaryTranslation<TEnum, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        TEnum? item,
        RecordType header,
        long length)
        where TEnum : struct, Enum, IConvertible
        where TReader : IMutagenReadStream
    {
        try
        {
            if (!item.HasValue) return;
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                transl.WriteValue(writer, item.Value, length);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }
}
