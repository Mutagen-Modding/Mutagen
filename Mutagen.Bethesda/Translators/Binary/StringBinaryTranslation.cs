using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class StringBinaryTranslation
    {
        public readonly static StringBinaryTranslation Instance = new StringBinaryTranslation();

        public void ParseInto(
            MutagenFrame frame,
            IHasItem<string> item)
        {
            if (Parse(
                frame,
                out string subItem))
            {
                item.Item = subItem;
            }
            else
            {
                item.Unset();
            }
        }

        public virtual bool Parse(
            MutagenFrame frame,
            out string item)
        {
            return Parse(
                frame: frame,
                parseWhole: true,
                item: out item);
        }

        public void ParseInto(
            MutagenFrame frame,
            IHasItem<string> item,
            bool parseWhole)
        {
            if (Parse(
                frame,
                parseWhole,
                out string subItem))
            {
                item.Item = subItem;
            }
            else
            {
                item.Unset();
            }
        }

        public virtual bool Parse(
            MutagenFrame frame,
            bool parseWhole,
            out string item)
        {
            if (parseWhole)
            {
                item = BinaryStringUtility.ProcessWholeToZString(frame.ReadSpan(checked((int)frame.Remaining)));
            }
            else
            {
                item = BinaryStringUtility.ParseUnknownLengthString(frame.Reader);
            }
            return true;
        }

        public void Write(
            MutagenWriter writer,
            string item)
        {
            Write(writer, item, binaryType: StringBinaryType.NullTerminate);
        }

        public void Write(
            MutagenWriter writer,
            string item,
            StringBinaryType binaryType)
        {
            WriteString(writer, item, binaryType);
        }

        public static void WriteString(
            MutagenWriter writer,
            string item,
            StringBinaryType binaryType)
        {
            switch (binaryType)
            {
                case StringBinaryType.Plain:
                    writer.Write(item);
                    break;
                case StringBinaryType.NullTerminate:
                    writer.Write(item);
                    writer.Write((byte)0);
                    break;
                case StringBinaryType.PrependLength:
                    writer.Write(item.Length);
                    writer.Write(item);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<string> item,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            this.Write(
                writer,
                item.Item,
                binaryType: binaryType);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<string> item,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                binaryType: binaryType);
        }

        public void Write(
            MutagenWriter writer,
            string item,
            RecordType header,
            bool nullable,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            if (item == null)
            {
                if (nullable)
                {
                    return;
                }
                throw new ArgumentException("Non optional string was null.");
            }
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                this.Write(
                    writer,
                    item,
                    binaryType: binaryType);
            }
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<string> item,
            RecordType header,
            bool nullable,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                header,
                nullable,
                binaryType: binaryType);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<string> item,
            RecordType header,
            bool nullable,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            this.Write(
                writer,
                item.Item,
                header,
                nullable,
                binaryType: binaryType);
        }

        public void Write(MutagenWriter writer, string item, long length)
        {
            if (length != item.Length)
            {
                throw new ArgumentException($"Expected length was {item.Length}, but was passed {length}.");
            }
            Write(writer, item, binaryType: StringBinaryType.NullTerminate);
        }
    }
}
