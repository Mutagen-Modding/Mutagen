using Loqui;
using Loqui.Internal;
using Noggog;
using Noggog.Notifying;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class EnumBinaryTranslation<E>
        where E : struct, Enum, IConvertible
    {
        public readonly static EnumBinaryTranslation<E> Instance = new EnumBinaryTranslation<E>();

        public void ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<E> item,
            ErrorMaskBuilder errorMask)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(
                        frame,
                        out E subItem,
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
            }
        }

        public bool Parse(
            MutagenFrame frame,
            out E item,
            ErrorMaskBuilder errorMask)
        {
            item = ParseValue(frame, errorMask);
            return true;
        }

        public void ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<E?> item,
            ErrorMaskBuilder errorMask)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(
                        frame,
                        out E? subItem,
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
            }
        }

        public bool Parse(
            MutagenFrame frame,
            out E? item,
            ErrorMaskBuilder errorMask)
        {
            item = ParseValue(frame, errorMask);
            return true;
        }

        public void Write(MutagenWriter writer, E item, long length, ErrorMaskBuilder errorMask)
        {
            WriteValue(writer, item, length);
        }

        public void Write(MutagenWriter writer, E? item, long length, ErrorMaskBuilder errorMask)
        {
            if (!item.HasValue)
            {
                return;
            }
            WriteValue(writer, item.Value, length);
        }

        public void Write(
            MutagenWriter writer,
            E? item,
            long length,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
        {
            if (!item.HasValue)
            {
                errorMask.ReportExceptionOrThrow(new NotImplementedException());
                return;
            }
            WriteValue(writer, item.Value, length);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<E?> item,
            long length,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                length,
                fieldIndex,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<E> item,
            long length,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                length,
                fieldIndex,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<E?> item,
            long length,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
        {
            this.Write(
                writer,
                item.Item,
                length,
                fieldIndex,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<E> item,
            long length,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
        {
            this.Write(
                writer,
                item.Item,
                length,
                fieldIndex,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            E? item,
            RecordType header,
            long length,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            if (item == null)
            {
                if (nullable)
                {
                    return;
                }
                errorMask.ReportExceptionOrThrow(new ArgumentException("Non optional string was null."));
                return;
            }
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                WriteValue(writer, item.Value, length);
            }
        }

        public void Write(
            MutagenWriter writer,
            E? item,
            RecordType header,
            long length,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    this.Write(
                        writer,
                        item,
                        header,
                        length,
                        nullable,
                        errorMask);
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<E?> item,
            RecordType header,
            long length,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                header,
                length,
                fieldIndex,
                nullable,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<E> item,
            RecordType header,
            long length,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                header,
                length,
                fieldIndex,
                nullable,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<E?> item,
            RecordType header,
            long length,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            this.Write(
                writer,
                item.Item,
                header,
                length,
                fieldIndex,
                nullable,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<E> item,
            RecordType header,
            long length,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            this.Write(
                writer,
                item.Item,
                header,
                length,
                fieldIndex,
                nullable,
                errorMask);
        }

        public E ParseValue(MutagenFrame reader, ErrorMaskBuilder errorMask)
        {
            int i;
            switch (reader.Remaining)
            {
                case 1:
                    i = reader.Reader.ReadUInt8();
                    break;
                case 2:
                    i = reader.Reader.ReadInt16();
                    break;
                case 4:
                    i = reader.Reader.ReadInt32();
                    break;
                default:
                    throw new NotImplementedException();
            }
            return EnumExt<E>.Convert(i);
        }

        public E ParseValue(MutagenFrame reader)
        {
            return ParseValue(reader, errorMask: null);
        }

        protected void WriteValue(MutagenWriter writer, E item, long length)
        {
            var i = item.ToInt32(null);
            switch (length)
            {
                case 1:
                    writer.Write((byte)i);
                    break;
                case 2:
                    writer.Write((ushort)i);
                    break;
                case 4:
                    writer.Write(i);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
