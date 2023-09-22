using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class EnumBinaryTranslation<TEnum, TReader, TWriter>
    where TEnum : struct, Enum, IConvertible
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly EnumBinaryTranslation<TEnum, TReader, TWriter> Instance = new();
    public static readonly UnderlyingType Underlying;
    public static readonly int EnumSize = Enums<TEnum>.Values.Count;
    public static readonly IReadOnlyList<TEnum> Values = Enums<TEnum>.Values;

    public enum UnderlyingType
    {
        UShort,
        Int,
        UInt,
        Long,
        ULong,
        Byte,
    }

    static EnumBinaryTranslation()
    {
        var underlying = Enum.GetUnderlyingType(typeof(TEnum));
        if (underlying == typeof(int))
        {
            Underlying = UnderlyingType.Int;
        }
        else if (underlying == typeof(uint))
        {
            Underlying = UnderlyingType.UInt;
        }
        else if (underlying == typeof(long))
        {
            Underlying = UnderlyingType.Long;
        }
        else if (underlying == typeof(ulong))
        {
            Underlying = UnderlyingType.ULong;
        }
        else if (underlying == typeof(byte))
        {
            Underlying = UnderlyingType.Byte;
        }
        else if (underlying == typeof(ushort))
        {
            Underlying = UnderlyingType.UShort;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public bool Parse(
        TReader reader,
        int length,
        out TEnum item)
    {
        item = Parse(reader, length);
        return true;
    }

    public TEnum Parse(TReader reader, long length)
    {
        return ParseValue(reader, length);
    }

    public bool Parse(
        TReader reader,
        out TEnum? item)
    {
        item = ParseValue(reader, reader.Remaining);
        return true;
    }

    private TEnum ParseValue(IBinaryReadStream reader, long length)
    {
        var i = length switch
        {
            1 => reader.ReadUInt8(),
            2 => reader.ReadInt16(),
            4 => reader.ReadInt32(),
            8 => reader.ReadInt64(),
            _ => throw new NotImplementedException(),
        };
        return Enums<TEnum>.Convert(i);
    }

    public TEnum ParseValue(TReader reader)
    {
        switch (reader.Remaining)
        {
            case 1:
                Enums<TEnum>.Convert(reader.ReadUInt8());
                break;
            case 2:
                Enums<TEnum>.Convert(reader.ReadInt16());
                break;
            case 4:
                Enums<TEnum>.Convert(reader.ReadInt32());
                break;
            case 8:
                Enums<TEnum>.Convert(reader.ReadInt64());
                break;
            default:
                throw new NotImplementedException();
        }
        return default;
    }

    public void Write(TWriter writer, TEnum item, long length)
    {
        WriteValue(writer, item, length);
    }

    public void Write(TWriter writer, int item, long length)
    {
        switch (length)
        {
            case 1:
                writer.Write((byte)item);
                break;
            case 2:
                writer.Write((ushort)item);
                break;
            case 4:
                writer.Write(item);
                break;
            case 8:
                writer.Write((long)item);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void Write(
        TWriter writer,
        TEnum? item,
        long length)
    {
        if (!item.HasValue)
        {
            throw new NotImplementedException();
        }
        WriteValue(writer, item.Value, length);
    }

    public void WriteValue(TWriter writer, TEnum item, long length)
    {
        long i;
        switch (Underlying)
        {
            case UnderlyingType.Int:
                i = item.ToInt32(null);
                break;
            case UnderlyingType.UInt:
                i = item.ToUInt32(null);
                break;
            case UnderlyingType.Long:
                i = item.ToInt64(null);
                break;
            case UnderlyingType.ULong:
                i = (long)item.ToUInt64(null);
                break;
            case UnderlyingType.Byte:
                i = (byte)item.ToByte(null);
                break;
            case UnderlyingType.UShort:
                i = (ushort)item.ToUInt16(null);
                break;
            default:
                throw new NotImplementedException();
        }
        switch (length)
        {
            case 1:
                writer.Write((byte)i);
                break;
            case 2:
                writer.Write((ushort)i);
                break;
            case 4:
                writer.Write((uint)i);
                break;
            case 8:
                writer.Write((ulong)i);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}