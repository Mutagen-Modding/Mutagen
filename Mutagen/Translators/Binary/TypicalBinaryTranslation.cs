using Loqui;
using Noggog;
using Noggog.Notifying;
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
        protected abstract T ParseBytes(byte[] bytes);

        protected abstract T ParseValue(MutagenFrame reader);

        public TryGet<T> Parse(MutagenFrame frame, bool doMasks, out Exception errorMask)
        {
            try
            {
                var parse = ParseValue(frame);
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

        public TryGet<T> Parse<M>(
            MutagenFrame frame,
            int fieldIndex,
            bool doMasks,
            Func<M> errorMask)
            where M : IErrorMask
        {
            var ret = this.Parse(
                frame,
                doMasks,
                out var ex);
            ErrorMask.HandleErrorMask(
                errorMask,
                doMasks,
                fieldIndex,
                ex);
            return ret;
        }

        protected abstract void WriteValue(MutagenWriter writer, T item);

        TryGet<T> IBinaryTranslation<T, Exception>.Parse(MutagenFrame reader, bool doMasks, out Exception errorMask)
        {
            return Parse(reader, doMasks: doMasks, errorMask: out errorMask);
        }

        public void Write(MutagenWriter writer, T item, bool doMasks, out Exception errorMask)
        {
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
        }

        public void Write<M>(
            MutagenWriter writer,
            T item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            bool doMasks,
            Func<M> errorMask)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item,
                header,
                nullable,
                doMasks,
                out var subMask);
            ErrorMask.HandleException(
                errorMask,
                doMasks,
                fieldIndex,
                subMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            bool doMasks,
            Func<M> errorMask)
            where M : IErrorMask
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                header,
                fieldIndex,
                nullable,
                doMasks,
                errorMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            T item,
            int fieldIndex,
            bool doMasks,
            Func<M> errorMask)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item,
                doMasks,
                out var subMask);
            ErrorMask.HandleException(
                errorMask,
                doMasks,
                fieldIndex,
                subMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T> item,
            int fieldIndex,
            bool doMasks,
            Func<M> errorMask)
            where M : IErrorMask
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                doMasks,
                errorMask);
        }

        void IBinaryTranslation<T, Exception>.Write(MutagenWriter writer, T item, ContentLength length, bool doMasks, out Exception maskObj)
        {
            Write(writer, item, doMasks, out maskObj);
        }

        public void Write(
            MutagenWriter writer,
            T item,
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
                    WriteValue(writer, item);
                    errorMask = null;
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
