using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Strings;
using Noggog;
using System;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public class StringBinaryTranslation

{
    public static readonly StringBinaryTranslation Instance = new();

    public virtual bool Parse(
        MutagenFrame reader,
        out string item)
    {
        return Parse(
            reader: reader,
            parseWhole: true,
            item: out item);
    }

    public bool Parse(
        MutagenFrame reader,
        bool parseWhole,
        out string item,
        StringBinaryType binaryType = StringBinaryType.NullTerminate)
    {
        item = Parse(reader, parseWhole: parseWhole, stringBinaryType: binaryType, encoding: reader.MetaData.Encodings.NonTranslated);
        return true;
    }

    public string Parse(
        MutagenFrame reader,
        bool parseWhole = true,
        StringBinaryType stringBinaryType = StringBinaryType.NullTerminate)
    {
        return Parse(reader, reader.MetaData.Encodings.NonTranslated, parseWhole, stringBinaryType);
    }

    public string Parse(
        MutagenFrame reader,
        IMutagenEncoding encoding,
        bool parseWhole = true,
        StringBinaryType stringBinaryType = StringBinaryType.NullTerminate)
    {
        switch (stringBinaryType)
        {
            case StringBinaryType.Plain:
            case StringBinaryType.NullTerminate:
                if (parseWhole)
                {
                    return BinaryStringUtility.ProcessWholeToZString(reader.ReadMemory(checked((int)reader.Remaining)), encoding);
                }
                else
                {
                    return BinaryStringUtility.ParseUnknownLengthString(reader.Reader, encoding);
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

    public TranslatedString Parse(
        MutagenFrame reader,
        StringsSource source,
        StringBinaryType stringBinaryType,
        bool parseWhole = true)
    {
        if (reader.MetaData.StringsLookup != null)
        {
            if (reader.Remaining != 4)
            {
                throw new ArgumentException($"String in Strings File format had unexpected length: {reader.Remaining} != 4");
            }
            uint key = reader.ReadUInt32();
            if (key == 0) return new TranslatedString(reader.MetaData.TargetLanguage, directString: null);
            return reader.MetaData.StringsLookup.CreateString(source, key, reader.MetaData.TargetLanguage);
        }
        else
        {
            return Parse(reader, reader.MetaData.Encodings.NonLocalized, parseWhole, stringBinaryType);
        }
    }

    public bool Parse(
        MutagenFrame reader,
        StringsSource source,
        StringBinaryType binaryType,
        out TranslatedString item,
        bool parseWhole = true)
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
            if (key == 0) return new TranslatedString(parsingBundle.TargetLanguage, directString: null);
            return parsingBundle.StringsLookup.CreateString(source, key, parsingBundle.TargetLanguage);
        }
        else
        {
            return BinaryStringUtility.ProcessWholeToZString(data, parsingBundle.Encodings.NonLocalized);
        }
    }

    public void Write(
        MutagenWriter writer,
        string item)
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
        StringBinaryType binaryType = StringBinaryType.NullTerminate)
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
        StringBinaryType binaryType = StringBinaryType.NullTerminate)
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
        StringBinaryType binaryType = StringBinaryType.NullTerminate)
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
}