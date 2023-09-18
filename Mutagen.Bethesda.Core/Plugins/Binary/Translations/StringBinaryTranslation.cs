using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Strings;
using Noggog;
using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Meta;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public sealed class StringBinaryTranslation
{
    public static readonly StringBinaryTranslation Instance = new();

    public bool Parse<TReader>(
        TReader reader,
        out string item)
        where TReader : IMutagenReadStream
    {
        return Parse(
            reader: reader,
            parseWhole: true,
            item: out item,
            binaryType: StringBinaryType.NullTerminate);
    }

    public bool Parse<TReader>(
        TReader reader,
        bool parseWhole,
        out string item,
        StringBinaryType binaryType)
        where TReader : IMutagenReadStream
    {
        item = Parse(reader, parseWhole: parseWhole, stringBinaryType: binaryType, encoding: reader.MetaData.Encodings.NonTranslated);
        return true;
    }

    public string Parse<TReader>(
        TReader reader,
        StringBinaryType stringBinaryType,
        bool parseWhole = true)
        where TReader : IMutagenReadStream
    {
        return Parse(reader, reader.MetaData.Encodings.NonTranslated, stringBinaryType, parseWhole);
    }

    public string Parse<TReader>(
        TReader reader,
        IMutagenEncoding encoding,
        StringBinaryType stringBinaryType,
        bool parseWhole)
        where TReader : IMutagenReadStream
    {
        switch (stringBinaryType)
        {
            case StringBinaryType.Plain:
                if (parseWhole)
                {
                    return BinaryStringUtility.ToZString(reader.ReadMemory(checked((int)reader.Remaining)), encoding);
                }
                else
                {
                    return BinaryStringUtility.ParseUnknownLengthString(reader, encoding);
                }
            case StringBinaryType.NullTerminate:
                if (parseWhole)
                {
                    return BinaryStringUtility.ProcessWholeToZString(reader.ReadMemory(checked((int)reader.Remaining)), encoding);
                }
                else
                {
                    return BinaryStringUtility.ParseUnknownLengthString(reader, encoding);
                }
            case StringBinaryType.PrependLength:
            {
                var len = reader.ReadInt32();
                return BinaryStringUtility.ToZString(reader.ReadMemory(len), encoding);
            }
            case StringBinaryType.PrependLengthUShort:
            {
                var len = reader.ReadInt16();
                return BinaryStringUtility.ToZString(reader.ReadMemory(len), encoding);
            }
            default:
                throw new NotImplementedException();
        }
    }

    public TranslatedString Parse<TReader>(
        TReader reader,
        StringsSource source,
        StringBinaryType stringBinaryType,
        bool parseWhole = true)
        where TReader : IMutagenReadStream
    {
        if (reader.MetaData.StringsLookup != null)
        {
            if (reader.Remaining != 4)
            {
                throw new ArgumentException($"String in Strings File format had unexpected length: {reader.Remaining} != 4");
            }
            uint key = reader.ReadUInt32();
            if (key == 0) return new TranslatedString(reader.MetaData.TranslatedTargetLanguage, directString: null);
            return reader.MetaData.StringsLookup.CreateString(source, key, reader.MetaData.TranslatedTargetLanguage);
        }
        else
        {
            return Parse(reader, reader.MetaData.Encodings.NonLocalized, stringBinaryType, parseWhole);
        }
    }

    public bool Parse(
        MutagenFrame reader,
        StringsSource source,
        StringBinaryType binaryType,
        out TranslatedString item,
        bool parseWhole)
    {
        item = Parse(reader, source, binaryType, parseWhole);
        return true;
    }

    public TranslatedString Parse(
        ReadOnlyMemorySlice<byte> data,
        StringsSource source,
        ParsingBundle parsingBundle)
    {
        if (parsingBundle.StringsLookup != null)
        {
            if (data.Length != 4)
            {
                throw new ArgumentException($"String in Strings File format had unexpected length: {data.Length} != 4");
            }
            uint key = BinaryPrimitives.ReadUInt32LittleEndian(data);
            if (key == 0) return new TranslatedString(parsingBundle.TranslatedTargetLanguage, directString: null);
            return parsingBundle.StringsLookup.CreateString(source, key, parsingBundle.TranslatedTargetLanguage);
        }
        else
        {
            return BinaryStringUtility.ProcessWholeToZString(data, parsingBundle.Encodings.NonLocalized);
        }
    }

    public string Parse(
        ReadOnlyMemorySlice<byte> data,
        IMutagenEncoding encoding,
        bool parseWhole = true)
    {
        if (parseWhole)
        {
            return BinaryStringUtility.ProcessWholeToZString(data, encoding);
        }
        else
        {
            return BinaryStringUtility.ParseUnknownLengthString(data, encoding);
        }
    }

    public void Write(
        MutagenWriter writer,
        string item)
    {
        BinaryStringUtility.Write(writer, item, binaryType: StringBinaryType.NullTerminate, encoding: writer.MetaData.Encodings.NonTranslated);
    }

    public void Write(
        MutagenWriter writer,
        ReadOnlySpan<char> item)
    {
        BinaryStringUtility.Write(writer, item, binaryType: StringBinaryType.NullTerminate, encoding: writer.MetaData.Encodings.NonTranslated);
    }

    public void WriteNullable(
        MutagenWriter writer,
        string? item)
    {
        if (item == null) return;
        writer.Write(item, binaryType: StringBinaryType.NullTerminate, writer.MetaData.Encodings.NonTranslated);
    }

    public void Write(
        MutagenWriter writer,
        string item,
        StringBinaryType binaryType)
    {
        writer.Write(item, binaryType, writer.MetaData.Encodings.NonTranslated);
    }

    public void Write(
        MutagenWriter writer,
        string item,
        RecordType header,
        StringBinaryType binaryType)
    {
        try
        {
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                writer.Write(
                    item,
                    encoding: writer.MetaData.Encodings.NonTranslated,
                    binaryType: binaryType);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }

    public void WriteNullable(
        MutagenWriter writer,
        string? item,
        RecordType header,
        StringBinaryType binaryType)
    {
        if (item == null) return;
        try
        {
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                writer.Write(
                    item,
                    encoding: writer.MetaData.Encodings.NonTranslated,
                    binaryType: binaryType);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }

    public void Write(
        MutagenWriter writer,
        ITranslatedStringGetter item,
        RecordType header,
        StringBinaryType binaryType,
        StringsSource source)
    {
        try
        {
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                Write(
                    writer,
                    item,
                    binaryType,
                    source);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }

    public void Write(
        MutagenWriter writer,
        ITranslatedStringGetter item,
        StringBinaryType binaryType,
        StringsSource source)
    {
        if (writer.MetaData.StringsWriter == null)
        {
            string? str;
            if (writer.MetaData.TargetLanguageOverride == null)
            {
                str = item.String ?? string.Empty;
            }
            else
            {
                if (!item.TryLookup(writer.MetaData.TargetLanguageOverride.Value, out str))
                {
                    str = string.Empty;
                }
            }
            writer.Write(
                str,
                encoding: writer.MetaData.Encodings.NonLocalized,
                binaryType: binaryType);
        }
        else
        {
            writer.Write(writer.MetaData.StringsWriter.Register(item, source));
        }
    }

    public void WriteNullable(
        MutagenWriter writer,
        ITranslatedStringGetter? item,
        RecordType header,
        StringBinaryType binaryType,
        StringsSource source)
    {
        if (item == null) return;
        try
        {
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                Write(writer, item, binaryType, source);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }

    public void WriteNullable(
        MutagenWriter writer,
        ITranslatedStringGetter? item,
        StringBinaryType binaryType,
        StringsSource source)
    {
        if (item == null) return;
        Write(writer, item, binaryType, source);
    }

    public void WriteNullable(
        MutagenWriter writer,
        string? item,
        StringBinaryType binaryType)
    {
        if (item == null) return;
        writer.Write(
            item,
            encoding: writer.MetaData.Encodings.NonTranslated,
            binaryType: binaryType);
    }

    public void Write(MutagenWriter writer, string item, long length)
    {
        if (length != item.Length)
        {
            throw new ArgumentException($"Expected length was {item.Length}, but was passed {length}.");
        }
        writer.Write(item, binaryType: StringBinaryType.NullTerminate, encoding: writer.MetaData.Encodings.NonTranslated);
    }

    public int ExtractManyUInt16PrependedStringsLength(int countLength, ReadOnlySpan<byte> data)
    {
        uint amount;
        switch (countLength)
        {
            case 1:
                amount = data[0];
                data = data.Slice(1);
                break;
            case 2:
                amount = BinaryPrimitives.ReadUInt16LittleEndian(data);
                data = data.Slice(2);
                break;
            case 4:
                amount = BinaryPrimitives.ReadUInt32LittleEndian(data);
                data = data.Slice(4);
                break;
            default:
                throw new ArgumentException();
        }

        int ret = 0;
        for (uint i = 0; i < amount; i++)
        {
            var len = ExtractUInt16PrependedStringsLength(data);
            ret += len;
            data = data.Slice(len);
        }

        return ret;
    }

    public int ExtractUInt16PrependedStringsLength(ReadOnlySpan<byte> data)
    {
        return 2 + BinaryPrimitives.ReadUInt16LittleEndian(data);
    }
}