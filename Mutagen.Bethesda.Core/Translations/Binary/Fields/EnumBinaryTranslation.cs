using Loqui.Internal;
using Noggog;
using System;

namespace Mutagen.Bethesda.Binary
{
    public class EnumBinaryTranslation<E>
        where E : struct, Enum, IConvertible
    {
        public readonly static EnumBinaryTranslation<E> Instance = new EnumBinaryTranslation<E>();
        public readonly static UnderlyingType Underlying;
        public readonly static int EnumSize = EnumExt.GetSize<E>();

        public enum UnderlyingType
        {
            Int,
            UInt,
            Long,
            ULong,
            Byte,
        }

        static EnumBinaryTranslation()
        {
            var underlying = Enum.GetUnderlyingType(typeof(E));
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
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool Parse(
            MutagenFrame frame,
            out E item)
        {
            item = ParseValue(frame);
            return true;
        }

        public E Parse(MutagenFrame frame)
        {
            return ParseValue(frame);
        }

        public bool Parse(
            MutagenFrame frame,
            out E? item)
        {
            item = ParseValue(frame);
            return true;
        }

        public void Write(MutagenWriter writer, E item, long length)
        {
            WriteValue(writer, item, length);
        }

        public void Write(MutagenWriter writer, int item, long length)
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
                default:
                    throw new NotImplementedException();
            }
        }

        public void Write(
            MutagenWriter writer,
            E? item,
            long length)
        {
            if (!item.HasValue)
            {
                throw new NotImplementedException();
            }
            WriteValue(writer, item.Value, length);
        }

        public void Write(
            MutagenWriter writer,
            E item,
            RecordType header,
            long length)
        {
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                WriteValue(writer, item, length);
            }
        }

        public void WriteNullable(
            MutagenWriter writer,
            E? item,
            RecordType header,
            long length)
        {
            if (!item.HasValue) return;
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                WriteValue(writer, item.Value, length);
            }
        }

        public E ParseValue(MutagenFrame reader, ErrorMaskBuilder? errorMask)
        {
            int i;
            switch (reader.Remaining)
            {
                case 1:
                    i = reader.Reader.ReadUInt8();
                    break;
                case 2:
                    i = reader.Reader.ReadInt16();
                    break;
                case 4:
                    i = reader.Reader.ReadInt32();
                    break;
                default:
                    throw new NotImplementedException();
            }
            return EnumExt<E>.Convert(i);
        }

        public E ParseValue(MutagenFrame reader)
        {
            return ParseValue(reader, errorMask: null);
        }

        protected void WriteValue(MutagenWriter writer, E item, long length)
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
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
