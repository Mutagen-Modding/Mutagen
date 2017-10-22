using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class EnumBinaryTranslation<E> : IBinaryTranslation<E, Exception>, IBinaryTranslation<E?, Exception>
        where E : struct, IComparable, IConvertible
    {
        public readonly static EnumBinaryTranslation<E> Instance = new EnumBinaryTranslation<E>();

        public TryGet<E> Parse(MutagenFrame frame, bool doMasks, out Exception errorMask)
        {
            try
            {
                var parse = ParseValue(frame);
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

        public void Write(
            MutagenWriter writer,
            E? item,
            RecordType header,
            ContentLength length,
            bool nullable,
            bool doMasks,
            out Exception errorMask)
        {
            if (item == null)
            {
                if (nullable)
                {
                    errorMask = null;
                    return;
                }
                throw new ArgumentException("Non optional string was null.");
            }
            try
            {
                using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
                {
                    WriteValue(writer, item.Value, length);
                }
                errorMask = null;
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = ex;
            }
        }

        protected E ParseValue(MutagenFrame reader)
        {
            int i;
            switch (reader.Length.Value)
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
            switch (length.Value)
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

        TryGet<E?> IBinaryTranslation<E?, Exception>.Parse(MutagenFrame frame, bool doMasks, out Exception maskObj)
        {
            return Parse(
                frame,
                doMasks,
                out maskObj).Bubble<E?>((t) => t);
        }
    }
}
