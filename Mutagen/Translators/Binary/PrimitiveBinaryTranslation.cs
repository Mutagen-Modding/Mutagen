using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public abstract class PrimitiveBinaryTranslation<T> : IBinaryTranslation<T, Exception>, IBinaryTranslation<T?, Exception>
        where T : struct
    {
        public abstract byte? ExpectedLength { get; }

        protected abstract T ParseValue(MutagenReader reader);

        public TryGet<T> Parse(MutagenReader reader, bool doMasks, out Exception errorMask)
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

        public TryGet<T> Parse(MutagenReader reader, ContentLength length, bool doMasks, out Exception errorMask)
        {
            if (this.ExpectedLength.HasValue && length != this.ExpectedLength)
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

        protected abstract void WriteValue(MutagenWriter writer, T item);

        TryGet<T?> IBinaryTranslation<T?, Exception>.Parse(MutagenReader reader, ContentLength length, bool doMasks, out Exception maskObj)
        {
            return Parse(
                reader,
                length,
                doMasks,
                out maskObj).Bubble<T?>((t) => t);
        }

        void IBinaryTranslation<T?, Exception>.Write(MutagenWriter writer, T? item, ContentLength length, bool doMasks, out Exception errorMask)
        {
            if (this.ExpectedLength.HasValue && length != this.ExpectedLength)
            {
                var ex = new ArgumentException($"Expected length was {this.ExpectedLength}, but was passed {length}.");
                if (doMasks)
                {
                    errorMask = ex;
                    return;
                }
                throw ex;
            }
            errorMask = Write_Internal(writer, item, doMasks);
        }

        public void Write(MutagenWriter writer, T? item, bool doMasks, out Exception errorMask)
        {
            errorMask = Write_Internal(writer, item, doMasks);
        }

        private Exception Write_Internal(MutagenWriter writer, T? item, bool doMasks)
        {
            Exception errorMask = null;
            if (!item.HasValue) return errorMask;
            try
            {
                WriteValue(writer, item.Value);
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = ex;
            }

            return errorMask;
        }

        private Exception Write_Internal(MutagenWriter writer, T item, bool doMasks)
        {
            Exception errorMask;
            try
            {
                WriteValue(writer, item);
                errorMask = null;
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = ex;
            }

            return errorMask;
        }

        void IBinaryTranslation<T, Exception>.Write(MutagenWriter writer, T item, ContentLength length, bool doMasks, out Exception errorMask)
        {
            if (this.ExpectedLength.HasValue && length != this.ExpectedLength)
            {
                var ex = new ArgumentException($"Expected length was {this.ExpectedLength}, but was passed {length}.");
                if (doMasks)
                {
                    errorMask = ex;
                    return;
                }
                throw ex;
            }
            errorMask = Write_Internal(writer, (T?)item, doMasks);
        }

        public void Write(
            MutagenWriter writer,
            T? item,
            RecordType header,
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
                    errorMask = this.Write_Internal(writer, item, doMasks);
                }
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = ex;
            }
        }
    }
}
