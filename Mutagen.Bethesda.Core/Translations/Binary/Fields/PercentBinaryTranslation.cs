using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class PercentBinaryTranslation
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

        public static Percent Parse(MutagenFrame frame, FloatIntegerType integerType)
        {
            switch (integerType)
            {
                case FloatIntegerType.UInt:
                    return Percent.FactoryPutInRange(((double)frame.ReadUInt32()) / uint.MaxValue);
                case FloatIntegerType.UShort:
                    return Percent.FactoryPutInRange(((double)frame.ReadUInt16()) / ushort.MaxValue);
                case FloatIntegerType.Byte:
                    return Percent.FactoryPutInRange(((double)frame.ReadUInt8()) / byte.MaxValue);
                default:
                    throw new NotImplementedException();
            }
        }

        public static void Write(MutagenWriter writer, Percent item, FloatIntegerType integerType)
        {
            switch (integerType)
            {
                case FloatIntegerType.UInt:
                    writer.Write((uint)(item * uint.MaxValue));
                    return;
                case FloatIntegerType.UShort:
                    writer.Write((ushort)(item * ushort.MaxValue));
                    return;
                case FloatIntegerType.Byte:
                    writer.Write((byte)(item * byte.MaxValue));
                    return;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
