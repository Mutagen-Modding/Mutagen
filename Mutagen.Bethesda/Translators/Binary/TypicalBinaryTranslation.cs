using Loqui;
using Loqui.Internal;
using Noggog;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public abstract class TypicalBinaryTranslation<T> : IBinaryTranslation<T>
        where T : class
    {
        protected abstract T ParseBytes(byte[] bytes);

        protected abstract T ParseValue(MutagenFrame reader);

        public void ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<T> item,
            ErrorMaskBuilder errorMask)
        {
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
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
            }
        }

        public bool Parse(MutagenFrame frame, out T item, ErrorMaskBuilder errorMask)
        {
            item = ParseValue(frame);
            if (item == null)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException("Value was unexpectedly null."));
                return false;
            }
            return true;
        }

        public abstract void Write(MutagenWriter writer, T item);

        public void Write(
            MutagenWriter writer,
            T item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
                    this.Write(
                        writer,
                        item,
                        header,
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
            T item,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
        {
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
                    this.Write(
                        writer,
                        item);
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
            IHasItemGetter<T> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask )
        {
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask);
        }

        void IBinaryTranslation<T>.Write(MutagenWriter writer, T item, long length, ErrorMaskBuilder errorMask)
        {
            Write(writer, item);
        }

        public void Write(
            MutagenWriter writer,
            T item,
            RecordType header,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            if (item == null)
            {
                if (nullable) return;
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException("Non optional string was null."));
            }
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                Write(writer, item);
            }
        }
    }
}
