using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public abstract class PrimitiveBinaryTranslation<T> : IBinaryTranslation<T, Exception>, IBinaryTranslation<T?, Exception>
        where T : struct
    {
        protected abstract T ParseBytes(byte[] bytes);

        public TryGet<T?> Parse(BinaryReader reader, bool doMasks, out Exception errorMask)
        {
            return Parse(reader, nullable: true, doMasks: doMasks, errorMask: out errorMask);
        }

        public TryGet<T> ParseNonNull(BinaryReader reader, bool doMasks, out Exception errorMask)
        {
            var parse = this.Parse(reader, nullable: false, doMasks: doMasks, errorMask: out errorMask);
            if (parse.Failed) return parse.BubbleFailure<T>();
            return TryGet<T>.Succeed(parse.Value.Value);
        }

        protected virtual T? ParseValue(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
        
        public TryGet<T?> Parse(BinaryReader reader, bool nullable, bool doMasks, out Exception errorMask)
        {
            try
            {
                var parse = ParseValue(reader);
                if (!nullable && !parse.HasValue)
                {
                    throw new ArgumentException("Value was unexpectedly null.");
                }
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

        protected virtual void WriteValue(BinaryWriter writer, string name, T? item)
        {
            throw new NotImplementedException();
        }

        TryGet<T> IBinaryTranslation<T, Exception>.Parse(BinaryReader reader, bool doMasks, out Exception errorMask)
        {
            return ParseNonNull(reader, doMasks, out errorMask);
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
