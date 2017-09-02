using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Binary
{
    public abstract class TypicalBinaryTranslation<T> : IBinaryTranslation<T, Exception>
        where T : class
    {
        protected virtual string GetItemStr(T item)
        {
            return item.ToStringSafe();
        }

        protected abstract T ParseBytes(byte[] bytes);

        protected virtual T ParseValue(BinaryReader reader, long length)
        {
            throw new NotImplementedException();
        }

        public TryGet<T> Parse(BinaryReader reader, long length, bool doMasks, out Exception errorMask)
        {
            try
            {
                var parse = ParseValue(reader, length);
                if (parse == null)
                {
                    throw new ArgumentException("Value was unexpectedly null.");
                }
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

        protected virtual void WriteValue(BinaryWriter writer, T item)
        {
            throw new NotImplementedException();
        }

        TryGet<T> IBinaryTranslation<T, Exception>.Parse(BinaryReader reader, long length, bool doMasks, out Exception errorMask)
        {
            return Parse(reader, length: length, doMasks: doMasks, errorMask: out errorMask);
        }

        private Exception Write_Internal(BinaryWriter writer, T item, bool doMasks, bool nullable)
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
            errorMask = Write_Internal(writer, item, doMasks, nullable: false);
        }
    }
}
