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
        public abstract ContentLength? ExpectedLength { get; }

        protected abstract T ParseValue(MutagenFrame reader);

        public TryGet<T> Parse(MutagenFrame frame, bool doMasks, out Exception errorMask)
        {
            try
            {
                if (ExpectedLength.HasValue)
                {
                    if (!frame.Reader.TryCheckUpcomingRead(this.ExpectedLength.Value, out var ex))
                    {
                        frame.Position = frame.FinalPosition;
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

        public TryGet<T> Parse(MutagenFrame frame, ContentLength length, bool doMasks, out Exception errorMask)
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
                throw new ArgumentException("Non optional item was null.");
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

        public void Write<M>(
            MutagenWriter writer,
            T? item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            Func<M> errorMask)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item,
                header,
                nullable,
                errorMask != null,
                out var subMask);
            ErrorMask.HandleException(
                errorMask,
                fieldIndex,
                subMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            T? item,
            int fieldIndex,
            Func<M> errorMask)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item,
                errorMask != null,
                out var subMask);
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
