using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public static class FloatBinaryTranslationExt
{
    public static void Write<TReader>(
        this PrimitiveBinaryTranslation<float, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        float item,
        RecordType header,
        float divisor)
        where TReader : IMutagenReadStream
    {
        transl.Write(writer, item / divisor, header);
    }

    public static void WriteNullable<TReader>(
        this PrimitiveBinaryTranslation<float, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        float? item,
        RecordType header,
        float divisor)
        where TReader : IMutagenReadStream
    {
        if (!item.HasValue) return;
        transl.Write(writer, item.Value / divisor, header);
    }

    public static void Write<TReader>(
        this PrimitiveBinaryTranslation<float, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        float? item,
        RecordType header,
        FloatIntegerType integerType,
        double divisor)
        where TReader : IMutagenReadStream
    {
        try
        {
            if (item == null) return;
            using (HeaderExport.Subrecord(writer, header))
            {
                FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(writer, item, integerType,
                    divisor: divisor);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }
}