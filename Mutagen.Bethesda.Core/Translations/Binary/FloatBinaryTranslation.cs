using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class FloatBinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<float, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly FloatBinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 4;

    public override float Parse(TReader reader)
    {
        var ret = reader.ReadFloat();
        if (ret == float.Epsilon)
        {
            return 0f;
        }
        return ret;
    }

    public float Parse(TReader reader, FloatIntegerType integerType, double multiplier)
    {
        switch (integerType)
        {
            case FloatIntegerType.UInt:
            {
                var raw = reader.ReadUInt32();
                double d = raw;
                d *= multiplier;
                return (float)d;
            }
            case FloatIntegerType.UShort:
            {
                var raw = reader.ReadUInt16();
                double d = raw;
                d *= multiplier;
                return (float)d;
            }
            case FloatIntegerType.Byte:
            {
                var raw = reader.ReadUInt8();
                double d = raw;
                d *= multiplier;
                return (float)d;
            }
            default:
                throw new NotImplementedException();
        }
    }

    public float Parse(TReader reader, float multiplier)
    {
        return Parse(reader) * multiplier;
    }

    public float GetFloat(ReadOnlySpan<byte> bytes, FloatIntegerType integerType, double multiplier)
    {
        switch (integerType)
        {
            case FloatIntegerType.UInt:
            {
                var raw = BinaryPrimitives.ReadUInt32LittleEndian(bytes);
                double d = raw;
                d *= multiplier;
                return (float)d;
            }
            case FloatIntegerType.UShort:
            {
                var raw = BinaryPrimitives.ReadUInt16LittleEndian(bytes);
                double d = raw;
                d *= multiplier;
                return (float)d;
            }
            case FloatIntegerType.Byte:
            {
                var raw = bytes[0];
                double d = raw;
                d *= multiplier;
                return (float)d;
            }
            default:
                throw new NotImplementedException();
        }
    }

    public override void Write(TWriter writer, float item)
    {
        if (item == float.Epsilon)
        {
            item = 0f;
        }
        if (item == 0f)
        {
            writer.Write(0);
        }
        else
        {
            writer.Write(item);
        }
    }

    public void Write(
        TWriter writer,
        float item,
        float divisor)
    {
        Write(writer, item / divisor);
    }

    public void Write(
        TWriter writer,
        float? item,
        FloatIntegerType integerType, 
        double divisor)
    {
        if (item == null) return;
        var unrounded = item.Value / divisor;
        switch (integerType)
        {
            case FloatIntegerType.UInt:
                writer.Write(checked((uint)Math.Round(unrounded)));
                break;
            case FloatIntegerType.UShort:
                writer.Write(checked((ushort)Math.Round(unrounded)));
                break;
            case FloatIntegerType.Byte:
                writer.Write(checked((byte)Math.Round(unrounded)));
                break;
            default:
                throw new NotImplementedException();
        }
    }
}