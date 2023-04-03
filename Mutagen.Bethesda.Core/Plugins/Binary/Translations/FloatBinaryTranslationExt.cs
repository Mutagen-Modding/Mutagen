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
        float? divisor, 
        float? multiplier)
        where TReader : IMutagenReadStream
    {
        try
        {
            using (HeaderExport.Subrecord(writer, header))
            {
                FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                    writer, item, 
                    multiplier: multiplier, divisor: divisor);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }

    public static void WriteNullable<TReader>(
        this PrimitiveBinaryTranslation<float, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        float? item,
        RecordType header,
        float? divisor, 
        float? multiplier)
        where TReader : IMutagenReadStream
    {
        try
        {
            if (item == null) return;
            using (HeaderExport.Subrecord(writer, header))
            {
                FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                    writer, item.Value, 
                    multiplier: multiplier, divisor: divisor);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }

    public static void Write<TReader>(
        this PrimitiveBinaryTranslation<float, TReader, MutagenWriter> transl,
        MutagenWriter writer,
        float? item,
        RecordType header,
        FloatIntegerType integerType,
        float? divisor = null, 
        float? multiplier = null)
        where TReader : IMutagenReadStream
    {
        try
        {
            if (item == null) return;
            using (HeaderExport.Subrecord(writer, header))
            {
                FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                    writer, item, integerType,
                    multiplier: multiplier, divisor: divisor);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }
}