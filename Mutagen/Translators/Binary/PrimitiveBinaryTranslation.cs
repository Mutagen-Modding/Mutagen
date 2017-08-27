using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public abstract class PrimitiveBinaryTranslation<T> : IBinaryTranslation<T, Exception>, IBinaryTranslation<T?, Exception>
        where T : struct
    {
        protected abstract T ParseBytes(byte[] bytes);

        protected virtual T ParseValue(BinaryReader reader, int length)
        {
            throw new NotImplementedException();
        }

        public TryGet<T> Parse(BinaryReader reader, int length, bool doMasks, out Exception errorMask)
        {
            try
            {
                var parse = ParseValue(reader, length);
                errorMask = null;
                return TryGet<T>.Succeed(parse);
            }
            catch (Exception ex)
            {
                if (doMasks)
                {
                    errorMask = ex;
                    return TryGet<T>.Failure;
                }
                throw;
            }
        }

        public TryGet<T?> Parse(BinaryReader reader, string header, int lengthLength, bool nullable, bool doMasks, out Exception errorMask)
        {
            try
            {
                if (!HeaderTranslation.TryParse(
                    reader, 
                    header,
                    lengthLength,
                    out int contentLength))
                {
                    if (!nullable)
                    {
                        throw new ArgumentException("Value was unexpectedly null.");
                    }
                    errorMask = null;
                    return TryGet<T?>.Succeed(default(T?));
                }
                var parse = ParseValue(reader, contentLength);
                errorMask = null;
                return TryGet<T?>.Succeed(parse);
            }
            catch (Exception ex)
            {
                if (doMasks)
                {
                    errorMask = ex;
                    return TryGet<T?>.Failure;
                }
                throw;
            }
        }

        protected virtual void WriteValue(Stream stream, string name, T? item)
        {
            throw new NotImplementedException();
        }

        TryGet<T> IBinaryTranslation<T, Exception>.Parse(BinaryReader reader, string header, int lengthLength, bool doMasks, out Exception maskObj)
        {
            return Parse(
                reader,
                header,
                lengthLength,
                nullable: false,
                doMasks: doMasks,
                errorMask: out maskObj).Bubble<T>((t) => t.Value);
        }

        TryGet<T?> IBinaryTranslation<T?, Exception>.Parse(BinaryReader reader, string header, int lengthLength, bool doMasks, out Exception maskObj)
        {
            return Parse(
                reader,
                header,
                lengthLength,
                nullable: true,
                doMasks: doMasks,
                errorMask: out maskObj);
        }

        TryGet<T?> IBinaryTranslation<T?, Exception>.Parse(BinaryReader reader, int length, bool doMasks, out Exception maskObj)
        {
            return Parse(
                reader,
                length,
                doMasks,
                out maskObj).Bubble<T?>((t) => t);
        }

        public void Write(BinaryWriter writer, T? item, bool doMasks, out Exception errorMask)
        {
            errorMask = Write_Internal(writer, item, doMasks, nullable: true);
        }

        private Exception Write_Internal(BinaryWriter writer, T? item, bool doMasks, bool nullable)
        {
            Exception errorMask;
            try
            {
                throw new NotImplementedException();
                errorMask = null;
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = ex;
            }

            return errorMask;
        }

        public void Write(BinaryWriter writer, T item, bool doMasks, out Exception errorMask)
        {
            errorMask = Write_Internal(writer, (T?)item, doMasks, nullable: false);
        }
    }
}
