using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public static class PercentBinaryTranslation
{
    public static Percent GetPercent(ReadOnlySpan<byte> bytes, FloatIntegerType integerType)
    {
        switch (integerType)
        {
            case FloatIntegerType.UInt:
                return Percent.FactoryPutInRange(((double)BinaryPrimitives.ReadUInt32LittleEndian(bytes)) / uint.MaxValue);
            case FloatIntegerType.UShort:
                return Percent.FactoryPutInRange(((double)BinaryPrimitives.ReadUInt16LittleEndian(bytes)) / ushort.MaxValue);
            case FloatIntegerType.Byte:
                return Percent.FactoryPutInRange(((double)bytes[0]) / byte.MaxValue);
            default:
                throw new NotImplementedException();
        }
    }

    public static Percent Parse<TReader>(TReader reader, FloatIntegerType integerType)
        where TReader : IMutagenReadStream
    {
        switch (integerType)
        {
            case FloatIntegerType.UInt:
                return Percent.FactoryPutInRange(((double)reader.ReadUInt32()) / uint.MaxValue);
            case FloatIntegerType.UShort:
                return Percent.FactoryPutInRange(((double)reader.ReadUInt16()) / ushort.MaxValue);
            case FloatIntegerType.Byte:
                return Percent.FactoryPutInRange(((double)reader.ReadUInt8()) / byte.MaxValue);
            default:
                throw new NotImplementedException();
        }
    }

    public static void Write(MutagenWriter writer, Percent? item, FloatIntegerType integerType)
    {
        if (!item.HasValue) return;
        switch (integerType)
        {
            case FloatIntegerType.UInt:
                writer.Write((uint)(item.Value * uint.MaxValue));
                return;
            case FloatIntegerType.UShort:
                writer.Write((ushort)(item.Value * ushort.MaxValue));
                return;
            case FloatIntegerType.Byte:
                writer.Write((byte)(item.Value * byte.MaxValue));
                return;
            default:
                throw new NotImplementedException();
        }
    }

    public static void Write(MutagenWriter writer, Percent? item, FloatIntegerType integerType, RecordType header)
    {
        if (!item.HasValue) return;
        using (HeaderExport.Subrecord(writer, header))
        {
            Write(writer, item, integerType);
        }
    }
}
