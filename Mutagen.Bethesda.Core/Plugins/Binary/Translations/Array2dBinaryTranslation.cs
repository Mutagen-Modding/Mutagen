using System.Data;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;
using static Mutagen.Bethesda.Plugins.Binary.Translations.PluginUtilityTranslation; 
using static Mutagen.Bethesda.Translations.Binary.UtilityTranslation;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

internal sealed class Array2dBinaryTranslation<T>
{
    public static readonly Array2dBinaryTranslation<T> Instance = new();

    public IArray2d<T> Parse<TReader>(
        TReader reader,
        P2Int size,
        BinarySubParseDelegate<TReader, T> transl)
        where TReader : IMutagenReadStream
    {
        var ret = new Array2d<T>(size, default!);
        for (int y = 0; y < size.Y; y++)
        {
            for (int x = 0; x < size.X; x++)
            {
                if (!transl(reader, out var item))
                {
                    throw new DataException("Expected to parse item");
                }

                ret[x, y] = item;
            }
        }

        return ret;
    }

    public IArray2d<T> Parse(
        MutagenFrame reader,
        P2Int size,
        BinaryMasterParseDelegate<T> transl,
        TypedParseParams translationParams = default)
    {
        var ret = new Array2d<T>(size, default!);
        for (int y = 0; y < size.Y; y++)
        {
            for (int x = 0; x < size.X; x++)
            {
                if (!transl(reader, out var item, translationParams))
                {
                    throw new DataException("Expected to parse item");
                }

                ret[x, y] = item;
            }
        }

        return ret;
    }

    public void Write(
        MutagenWriter writer,
        IReadOnlyArray2d<T>? items,
        RecordType recordType,
        BinarySubWriteDelegate<MutagenWriter, T> transl)
    {
        if (items == null) return;
        try
        {
            using (HeaderExport.Subrecord(writer, recordType))
            {
                for (int y = 0; y < items.Height; y++)
                {
                    for (int x = 0; x < items.Width; x++)
                    {
                        transl(writer, items[x, y]);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, recordType);
            throw;
        }
    }

    public void Write(
        MutagenWriter writer,
        IReadOnlyArray2d<T>? items,
        RecordType recordType,
        BinaryMasterWriteDelegate<T> transl, 
        TypedWriteParams translationParams = default)
    {
        if (items == null) return;
        try
        {
            using (HeaderExport.Subrecord(writer, recordType))
            {
                for (int y = 0; y < items.Height; y++)
                {
                    for (int x = 0; x < items.Width; x++)
                    {
                        transl(writer, items[x, y], translationParams);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, recordType);
            throw;
        }
    }

    public void Write(
        MutagenWriter writer,
        IReadOnlyArray2d<T> items,
        BinarySubWriteDelegate<MutagenWriter, T> transl)
    {
        for (int y = 0; y < items.Height; y++)
        {
            for (int x = 0; x < items.Width; x++)
            {
                transl(writer, items[x, y]);
            }
        }
    }
}