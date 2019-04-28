using Loqui;
using Loqui.Internal;
using Noggog;
using Noggog.Notifying;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public interface IPrimitiveBinaryTranslation
    {
        int ExpectedLength { get; }
    }

    public abstract class PrimitiveBinaryTranslation<T> : IPrimitiveBinaryTranslation
        where T : struct
    {
        public abstract int ExpectedLength { get; }

        public abstract T ParseValue(MutagenFrame reader);

        public void ParseInto(
            MutagenFrame frame,
            IHasItem<T> item)
        {
            if (Parse(
                frame,
                out T subItem))
            {
                item.Item = subItem;
            }
            else
            {
                item.Unset();
            }
        }

        public T Parse(MutagenFrame frame)
        {
            if (Parse(frame, out var item))
            {
                return item;
            }
            return default(T);
        }

        public bool Parse(MutagenFrame frame, out T item)
        {
            if (!frame.TryCheckUpcomingRead(this.ExpectedLength, out var ex))
            {
                throw ex;
            }
            item = ParseValue(frame);
            return true;
        }

        public void ParseInto(
            MutagenFrame frame,
            long length,
            IHasItem<T> item)
        {
            if (Parse(
                frame,
                length,
                out T subItem))
            {
                item.Item = subItem;
            }
            else
            {
                item.Unset();
            }
        }

        public bool Parse(MutagenFrame frame, long length, out T item)
        {
            if (length != this.ExpectedLength)
            {
                throw new ArgumentException($"Expected length was {this.ExpectedLength}, but was passed {length}.");
            }
            return Parse(frame, out item);
        }

        public abstract void WriteValue(MutagenWriter writer, T item);

        public void Write(MutagenWriter writer, T? item)
        {
            WriteValue(writer, item);
        }

        public void Write(MutagenWriter writer, T item)
        {
            WriteValue(writer, item);
        }

        public void WriteValue(MutagenWriter writer, T? item)
        {
            if (!item.HasValue) return;
            WriteValue(
                writer,
                item.Value);
        }

        public void Write(
            MutagenWriter writer,
            T? item,
            RecordType header,
            bool nullable,
            Action<MutagenWriter, T?> write = null)
        {
            if (item == null)
            {
                throw new ArgumentException("Non optional item was null.");
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
            Action<MutagenWriter, T?> write = null)
        {
            write = write ?? WriteValue;
            write(writer, item);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T> item)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T> item,
            RecordType header,
            bool nullable)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                header,
                nullable);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T?> item,
            RecordType header,
            bool nullable)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                header,
                nullable);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T?> item)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<T> item)
        {
            this.Write(
                writer,
                item.Item);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<T> item,
            RecordType header,
            bool nullable)
        {
            this.Write(
                writer,
                item.Item,
                header,
                nullable);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<T?> item,
            RecordType header,
            bool nullable)
        {
            this.Write(
                writer,
                item.Item,
                header,
                nullable);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<T?> item)
        {
            this.Write(
                writer,
                item.Item);
        }
    }
}
