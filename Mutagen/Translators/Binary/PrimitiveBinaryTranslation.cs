using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public abstract class PrimitiveBinaryTranslation<T> : IBinaryTranslation<T, Exception>, IBinaryTranslation<T?, Exception>
        where T : struct
    {
        public abstract byte ExpectedLength { get; }

        protected abstract T ParseValue(BinaryReader reader);

        public TryGet<T> Parse(BinaryReader reader, bool doMasks, out Exception errorMask)
        {
            try
            {
                var parse = ParseValue(reader);
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

        public TryGet<T> Parse(BinaryReader reader, int length, bool doMasks, out Exception errorMask)
        {
            if (length != this.ExpectedLength)
            {
                var ex = new ArgumentException($"Expected length was {this.ExpectedLength}, but was passed {length}.");
                if (doMasks)
                {
                    errorMask = ex;
                    return TryGet<T>.Failure;
                }
                throw ex;
            }
            return Parse(reader, doMasks, out errorMask);
        }

        protected virtual void WriteValue(Stream stream, string name, T? item)
        {
            throw new NotImplementedException();
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
