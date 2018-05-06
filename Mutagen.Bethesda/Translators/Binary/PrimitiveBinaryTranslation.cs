using Loqui;
using Noggog;
using Noggog.Notifying;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public abstract class PrimitiveBinaryTranslation<T> : IBinaryTranslation<T, Exception>, IBinaryTranslation<T?, Exception>
        where T : struct
    {
        public abstract int? ExpectedLength { get; }

        protected abstract T ParseValue(MutagenFrame reader);

        public TryGet<T> Parse(MutagenFrame frame, bool doMasks, out Exception errorMask)
        {
            try
            {
                if (ExpectedLength.HasValue)
                {
                    if (!frame.TryCheckUpcomingRead(this.ExpectedLength.Value, out var ex))
                    {
                        frame.Position = frame.FinalLocation;
                        throw ex;
                    }
                }
                var parse = ParseValue(frame);
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
            Func<M> errorMask)
            where M : IErrorMask
        {
            var ret = this.Parse(
                frame,
                errorMask != null,
                out var ex);
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                ex);
            return ret;
        }

        public TryGet<T> Parse(MutagenFrame frame, long length, bool doMasks, out Exception errorMask)
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
            return Parse(frame, doMasks, out errorMask);
        }

        protected abstract void WriteValue(MutagenWriter writer, T item);

        TryGet<T?> IBinaryTranslation<T?, Exception>.Parse(MutagenFrame frame, bool doMasks, out Exception maskObj)
        {
            return Parse(
                frame,
                doMasks,
                out maskObj).Bubble<T?>((t) => t);
        }

        void IBinaryTranslation<T?, Exception>.Write(MutagenWriter writer, T? item, long length, bool doMasks, out Exception errorMask)
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

        protected Exception Write_Internal(MutagenWriter writer, T? item, bool doMasks)
        {
            if (!item.HasValue) return null;
            return Write_Internal(
                writer,
                item.Value,
                doMasks);
        }

        protected Exception Write_Internal(MutagenWriter writer, T item, bool doMasks)
        {
            try
            {
                WriteValue(writer, item);
            }
            catch (Exception ex)
            when (doMasks)
            {
                return ex;
            }
            return null;
        }

        void IBinaryTranslation<T, Exception>.Write(MutagenWriter writer, T item, long length, bool doMasks, out Exception errorMask)
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
            out Exception errorMask,
            Func<MutagenWriter, T?, bool, Exception> write = null)
        {
            if (item == null)
            {
                if (nullable)
                {
                    errorMask = null;
                    return;
                }
                throw new ArgumentException("Non optional item was null.");
            }
            try
            {
                write = write ?? this.Write_Internal;
                using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
                {
                    errorMask = write(writer, item, doMasks);
                }
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = ex;
            }
        }

        public void Write<M>(
            MutagenWriter writer,
            T? item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            Func<M> errorMask,
            Func<MutagenWriter, T?, bool, Exception> write = null)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item,
                header,
                nullable,
                errorMask != null,
                out var subMask,
                write: write);
            ErrorMask.HandleException(
                errorMask,
                fieldIndex,
                subMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            T? item,
            int fieldIndex,
            Func<M> errorMask,
            Func<MutagenWriter, T?, bool, Exception> write = null)
            where M : IErrorMask
        {
            write = write ?? Write_Internal;
            var subMask = write(writer, item, errorMask != null);
            ErrorMask.HandleException(
                errorMask,
                fieldIndex,
                subMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T> item,
            int fieldIndex,
            Func<M> errorMask)
            where M : IErrorMask
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
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
                errorMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T?> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
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
                errorMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T?> item,
            int fieldIndex,
            Func<M> errorMask)
            where M : IErrorMask
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasItemGetter<T> item,
            int fieldIndex,
            Func<M> errorMask)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasItemGetter<T> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            Func<M> errorMask)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item.Item,
                header,
                fieldIndex,
                nullable,
                errorMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasItemGetter<T?> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            Func<M> errorMask)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item.Item,
                header,
                fieldIndex,
                nullable,
                errorMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasItemGetter<T?> item,
            int fieldIndex,
            Func<M> errorMask)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask);
        }
        }
}
