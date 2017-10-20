using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class EnumBinaryTranslation<E> : IBinaryTranslation<E, Exception>, IBinaryTranslation<E?, Exception>
        where E : struct, IComparable, IConvertible
    {
        public readonly static EnumBinaryTranslation<E> Instance = new EnumBinaryTranslation<E>();

        public TryGet<E> Parse(MutagenFrame reader, ContentLength length, bool doMasks, out Exception errorMask)
        {
            try
            {
                var parse = ParseValue(reader, length);
                errorMask = null;
                return TryGet<E>.Succeed(parse);
            }
            catch (Exception ex)
            {
                if (doMasks)
                {
                    errorMask = ex;
                    return TryGet<E>.Failure;
                }
                throw;
            }
        }

        public void Write(MutagenWriter writer, E item, ContentLength length, bool doMasks, out Exception errorMask)
        {
            Write(writer, (E?)item, length, doMasks, out errorMask);
        }

        public void Write(MutagenWriter writer, E? item, ContentLength length, bool doMasks, out Exception errorMask)
        {
            if (!item.HasValue)
            {
                errorMask = null;
                return;
            }
            try
            {
                WriteValue(writer, item.Value, length);
                errorMask = null;
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = ex;
            }
        }

        protected E ParseValue(MutagenFrame reader, ContentLength length)
        {
            int i;
            switch (length.Length)
            {
                case 1:
                    i = reader.Reader.ReadByte();
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
            return (E)Enum.ToObject(typeof(E), i);
        }

        protected void WriteValue(MutagenWriter writer, E item, ContentLength length)
        {
            switch (length.Length)
            {
                case 1:
                    writer.Write(item.ToByte(null));
                    break;
                case 2:
                    writer.Write(item.ToInt16(null));
                    break;
                case 4:
                    writer.Write(item.ToInt32(null));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        TryGet<E?> IBinaryTranslation<E?, Exception>.Parse(MutagenFrame reader, ContentLength length, bool doMasks, out Exception maskObj)
        {
            return Parse(
                reader,
                length,
                doMasks,
                out maskObj).Bubble<E?>((t) => t);
        }
    }
}
