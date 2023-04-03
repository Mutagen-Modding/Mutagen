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

    public float Parse(TReader reader, FloatIntegerType integerType,
        float? multiplier = null,
        float? divisor = null)
    {
        switch (integerType)
        {
            case FloatIntegerType.UInt:
            {
                var raw = reader.ReadUInt32();
                return ApplyTransformations(raw, multiplier: multiplier, divisor: divisor);
            }
            case FloatIntegerType.UShort:
            {
                var raw = reader.ReadUInt16();
                return ApplyTransformations(raw, multiplier: multiplier, divisor: divisor);
            }
            case FloatIntegerType.Byte:
            {
                var raw = reader.ReadUInt8();
                return ApplyTransformations(raw, multiplier: multiplier, divisor: divisor);
            }
            default:
                throw new NotImplementedException();
        }
    }

    public float Parse(TReader reader,
        float? multiplier,
        float? divisor)
    {
        return ApplyTransformations(Parse(reader), multiplier: multiplier, divisor: divisor);
    }

    public float GetFloat(
        ReadOnlySpan<byte> bytes,
        FloatIntegerType integerType,
        float? multiplier = null,
        float? divisor = null)
    {
        switch (integerType)
        {
            case FloatIntegerType.UInt:
            {
                var raw = BinaryPrimitives.ReadUInt32LittleEndian(bytes);
                return ApplyTransformations(raw, multiplier: multiplier, divisor: divisor);
            }
            case FloatIntegerType.UShort:
            {
                var raw = BinaryPrimitives.ReadUInt16LittleEndian(bytes);
                return ApplyTransformations(raw, multiplier: multiplier, divisor: divisor);
            }
            case FloatIntegerType.Byte:
            {
                var raw = bytes[0];
                return ApplyTransformations(raw, multiplier: multiplier, divisor: divisor);
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
        float? multiplier,
        float? divisor)
    {
        var transformed = ApplyTransformations(item, multiplier: multiplier, divisor: divisor);

        Write(writer, transformed);
    }

    public void Write(
        TWriter writer,
        float? item,
        FloatIntegerType integerType, 
        float? multiplier = null,
        float? divisor = null)
    {
        if (item == null) return;

        var transformed = ApplyTransformations(item.Value, multiplier: multiplier, divisor: divisor);
        
        switch (integerType)
        {
            case FloatIntegerType.UInt:
                writer.Write(checked((uint)Math.Round(transformed)));
                break;
            case FloatIntegerType.UShort:
                writer.Write(checked((ushort)Math.Round(transformed)));
                break;
            case FloatIntegerType.Byte:
                writer.Write(checked((byte)Math.Round(transformed)));
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public float ApplyTransformations(float input, float? multiplier, float? divisor)
    {
        if (multiplier == null && divisor == null)
        {
            return input;
        }

        if (multiplier == null)
        {
            return input / divisor!.Value;
        }

        if (divisor == null)
        {
            return input * multiplier.Value;
        }

        return input * multiplier.Value / divisor.Value;
    }
}