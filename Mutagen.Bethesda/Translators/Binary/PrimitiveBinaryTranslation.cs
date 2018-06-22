using Loqui;
using Loqui.Internal;
using Noggog;
using Noggog.Notifying;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public abstract class PrimitiveBinaryTranslation<T> : IBinaryTranslation<T>, IBinaryTranslation<T?>
        where T : struct
    {
        public abstract int? ExpectedLength { get; }

        protected abstract T ParseValue(MutagenFrame reader);

        public void ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<T> item,
            ErrorMaskBuilder errorMask)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (Parse(
                    frame,
                    out T subItem,
                    errorMask))
                {
                    item.Item = subItem;
                }
                else
                {
                    item.Unset();
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public T Parse(MutagenFrame frame, ErrorMaskBuilder errorMask)
        {
            if (Parse(frame, out var item, errorMask))
            {
                return item;
            }
            return default(T);
        }

        public bool Parse(MutagenFrame frame, out T item, ErrorMaskBuilder errorMask)
        {
            try
            {
                if (ExpectedLength.HasValue)
                {
                    if (!frame.TryCheckUpcomingRead(this.ExpectedLength.Value, out var ex))
                    {
                        frame.Position = frame.FinalLocation;
                        errorMask.ReportExceptionOrThrow(ex);
                        item = default(T);
                        return false;
                    }
                }
                item = ParseValue(frame);
                return true;
            }
            finally
            {
                frame.Dispose();
            }
        }

        public void ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            long length,
            IHasItem<T> item,
            ErrorMaskBuilder errorMask)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (Parse(
                    frame,
                    length,
                    out T subItem,
                    errorMask))
                {
                    item.Item = subItem;
                }
                else
                {
                    item.Unset();
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public bool Parse(MutagenFrame frame, long length, out T item, ErrorMaskBuilder errorMask)
        {
            if (this.ExpectedLength.HasValue && length != this.ExpectedLength)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"Expected length was {this.ExpectedLength}, but was passed {length}."));
                item = default(T);
                return false;
            }
            return Parse(frame, out item, errorMask);
        }

        protected abstract void WriteValue(MutagenWriter writer, T item);

        bool IBinaryTranslation<T?>.Parse(MutagenFrame frame, out T? item, ErrorMaskBuilder errorMask)
        {
            if (Parse(
                frame,
                out T subItem,
                errorMask))
            {
                item = subItem;
                return true;
            }

            item = default(T?);
            return false;
        }

        void IBinaryTranslation<T?>.Write(MutagenWriter writer, T? item, long length, ErrorMaskBuilder errorMask)
        {
            if (this.ExpectedLength.HasValue && length != this.ExpectedLength)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"Expected length was {this.ExpectedLength}, but was passed {length}."));
            }
            WriteValue(writer, item);
        }

        public void Write(MutagenWriter writer, T? item, ErrorMaskBuilder errorMask)
        {
            WriteValue(writer, item);
        }

        public void Write(MutagenWriter writer, T item, ErrorMaskBuilder errorMask)
        {
            WriteValue(writer, item);
        }

        protected void WriteValue(MutagenWriter writer, T? item)
        {
            if (!item.HasValue) return;
            WriteValue(
                writer,
                item.Value);
        }

        void IBinaryTranslation<T>.Write(MutagenWriter writer, T item, long length, ErrorMaskBuilder errorMask)
        {
            if (this.ExpectedLength.HasValue && length != this.ExpectedLength)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"Expected length was {this.ExpectedLength}, but was passed {length}."));
            }
            WriteValue(writer, (T?)item);
        }

        public void Write(
            MutagenWriter writer,
            T? item,
            RecordType header,
            bool nullable,
            ErrorMaskBuilder errorMask,
            Action<MutagenWriter, T?> write = null)
        {
            if (item == null)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException("Non optional item was null."));
            }
            write = write ?? this.WriteValue;
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                write(writer, item);
            }
        }

        public void Write(
            MutagenWriter writer,
            T? item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask,
            Action<MutagenWriter, T?> write = null)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                this.Write(
                    writer,
                    item,
                    header,
                    nullable,
                    errorMask,
                    write: write);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void Write(
            MutagenWriter writer,
            T? item,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            Action<MutagenWriter, T?> write = null)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                write = write ?? WriteValue;
                write(writer, item);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
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

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T?> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
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

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T?> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<T> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
        {
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<T> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            this.Write(
                writer,
                item.Item,
                header,
                fieldIndex,
                nullable,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<T?> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            this.Write(
                writer,
                item.Item,
                header,
                fieldIndex,
                nullable,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<T?> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
        {
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask);
        }
    }
}
