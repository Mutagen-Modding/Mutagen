using Noggog;
using System;
using System.Buffers.Binary;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class FloatBinaryTranslation : PrimitiveBinaryTranslation<float>
    {
        public readonly static FloatBinaryTranslation Instance = new FloatBinaryTranslation();
        public override int ExpectedLength => 4;

        public override float ParseValue(MutagenFrame reader)
        {
            var ret = reader.Reader.ReadFloat();
            if (ret == float.Epsilon)
            {
                return 0f;
            }
            return ret;
        }

        public static float Parse(MutagenFrame frame, FloatIntegerType integerType, float multiplier)
        {
            switch (integerType)
            {
                case FloatIntegerType.UInt:
                    {
                        var raw = frame.ReadUInt32();
                        float f = raw;
                        f *= multiplier;
                        return f;
                    }
                case FloatIntegerType.UShort:
                    {
                        var raw = frame.ReadUInt16();
                        float f = raw;
                        f *= multiplier;
                        return f;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public static float GetFloat(ReadOnlySpan<byte> bytes, FloatIntegerType integerType, float multiplier)
        {
            switch (integerType)
            {
                case FloatIntegerType.UInt:
                    {
                        var raw = BinaryPrimitives.ReadUInt32LittleEndian(bytes);
                        float f = raw;
                        f *= multiplier;
                        return f;
                    }
                case FloatIntegerType.UShort:
                    {
                        var raw = BinaryPrimitives.ReadUInt16LittleEndian(bytes);
                        float f = raw;
                        f *= multiplier;
                        return f;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public override void Write(MutagenWriter writer, float item)
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

        public static void Write(MutagenWriter writer, float? item, FloatIntegerType integerType, float multiplier)
        {
            if (item == null) return;
            switch (integerType)
            {
                case FloatIntegerType.UInt:
                    writer.Write((uint)Math.Round(item.Value / multiplier));
                    break;
                case FloatIntegerType.UShort:
                    writer.Write((ushort)Math.Round(item.Value / multiplier));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void Write(MutagenWriter writer, float? item, RecordType header, FloatIntegerType integerType, float multiplier)
        {
            if (item == null) return;
            using (HeaderExport.ExportSubrecordHeader(writer, header))
            {
                Write(writer, item, integerType, multiplier);
            }
        }
    }
}
