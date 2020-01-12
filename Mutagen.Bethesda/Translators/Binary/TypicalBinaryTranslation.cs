using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public abstract class TypicalBinaryTranslation<T>
        where T : class
    {
        protected abstract T ParseBytes(byte[] bytes);

        protected abstract T ParseValue(MutagenFrame reader);

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

        public bool Parse(MutagenFrame frame, out T item)
        {
            item = ParseValue(frame);
            if (item == null)
            {
                throw new ArgumentException("Value was unexpectedly null.");
            }
            return true;
        }

        public abstract void Write(MutagenWriter writer, T item);

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
            IHasBeenSetItemGetter<T> item)
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
            T item,
            RecordType header,
            bool nullable)
        {
            if (item == null)
            {
                if (nullable) return;
                throw new ArgumentException("Non optional string was null.");
            }
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                Write(writer, item);
            }
        }
    }
}
