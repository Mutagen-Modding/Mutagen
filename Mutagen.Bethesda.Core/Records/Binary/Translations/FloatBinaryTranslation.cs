using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Records.Binary.Streams;
using System;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Records.Binary.Translations
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

        public static float Parse(MutagenFrame frame, FloatIntegerType integerType, double multiplier)
        {
            switch (integerType)
            {
                case FloatIntegerType.UInt:
                    {
                        var raw = frame.ReadUInt32();
                        double d = raw;
                        d *= multiplier;
                        return (float)d;
                    }
                case FloatIntegerType.UShort:
                    {
                        var raw = frame.ReadUInt16();
                        double d = raw;
                        d *= multiplier;
                        return (float)d;
                    }
                case FloatIntegerType.Byte:
                    {
                        var raw = frame.ReadUInt8();
                        double d = raw;
                        d *= multiplier;
                        return (float)d;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public float Parse(MutagenFrame frame, float multiplier)
        {
            return Parse(frame) * multiplier;
        }

        public static float GetFloat(ReadOnlySpan<byte> bytes, FloatIntegerType integerType, double multiplier)
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

        public void Write(MutagenWriter writer, float item, float multiplier)
        {
            Write(writer, item / multiplier);
        }

        public void Write(MutagenWriter writer, float item, RecordType header, float multiplier)
        {
            Write(writer, item / multiplier, header);
        }

        public void WriteNullable(MutagenWriter writer, float? item, float multiplier)
        {
            if (!item.HasValue) return;
            Write(writer, item.Value / multiplier);
        }

        public void WriteNullable(MutagenWriter writer, float? item, RecordType header, float multiplier)
        {
            if (!item.HasValue) return;
            Write(writer, item.Value / multiplier, header);
        }

        public static void Write(MutagenWriter writer, float? item, FloatIntegerType integerType, double multiplier)
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
                case FloatIntegerType.Byte:
                    writer.Write((byte)Math.Round(item.Value / multiplier));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void Write(MutagenWriter writer, float? item, RecordType header, FloatIntegerType integerType, double multiplier)
        {
            try
            {
                if (item == null) return;
                using (HeaderExport.Subrecord(writer, header))
                {
                    Write(writer, item, integerType, multiplier);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, header);
            }
        }
    }
}
