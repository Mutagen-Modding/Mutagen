using Loqui;
using Loqui.Internal;
using Noggog;
using Noggog.Notifying;
using System;
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
            int fieldIndex,
            IHasItem<string> item,
            ErrorMaskBuilder errorMask)
        {
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(
                        frame,
                        out string subItem,
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

        public virtual bool Parse(
            MutagenFrame frame,
            out string item,
            ErrorMaskBuilder errorMask)
        {
            return Parse(
                frame: frame,
                parseWhole: true,
                item: out item,
                errorMask: errorMask);
        }

        public void ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<string> item,
            bool parseWhole,
            ErrorMaskBuilder errorMask)
        {
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(
                        frame,
                        parseWhole,
                        out string subItem,
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

        public virtual bool Parse(
            MutagenFrame frame,
            bool parseWhole,
            out string item,
            ErrorMaskBuilder errorMask)
        {
            errorMask = null;
            if (parseWhole)
            {
                item = frame.Reader.ReadString(checked((int)frame.Remaining));
                item = item.TrimEnd('\0');
            }
            else
            {
                item = ReadStringUntil(frame.Reader, '\0', include: false);
                frame.Reader.Position += 1;
            }
            return true;
        }

        public static string ReadStringUntil(
            IBinaryReadStream reader,
            char stopChar,
            bool include)
        {
            List<byte> chars = new List<byte>();
            while (!reader.Complete)
            {
                var nextChar = reader.ReadUInt8();
                if (nextChar == stopChar)
                {
                    if (include)
                    {
                        chars.Add(nextChar);
                    }
                    else
                    {
                        reader.Position -= 1;
                    }
                    break;
                }
                chars.Add(nextChar);
            }
            return BinaryUtility.BytesToString(chars.ToArray());
        }

        public void Write(
            MutagenWriter writer,
            string item,
            bool nullTerminate)
        {
            writer.Write(item);
            if (nullTerminate)
            {
                writer.Write((byte)0);
            }
        }

        public void Write(
            MutagenWriter writer,
            string item)
        {
            Write(writer, item, nullTerminate: true);
        }

        public void Write(
            MutagenWriter writer,
            string item,
            ErrorMaskBuilder errorMask)
        {
            Write(writer, item, nullTerminate: true);
        }

        public void Write(
            MutagenWriter writer,
            string item,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            bool nullTerminate = true)
        {
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
                    this.Write(
                        writer,
                        item,
                        nullTerminate: nullTerminate);
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
            IHasItemGetter<string> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            bool nullTerminate = true)
        {
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
                    this.Write(
                        writer,
                        item.Item,
                        nullTerminate: nullTerminate);
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
            IHasBeenSetItemGetter<string> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            bool nullTerminate = true)
        {
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
                    if (!item.HasBeenSet) return;
                    this.Write(
                        writer,
                        item.Item,
                        nullTerminate: nullTerminate);
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
            string item,
            RecordType header,
            bool nullable,
            ErrorMaskBuilder errorMask,
            bool nullTerminate = true)
        {
            if (item == null)
            {
                if (nullable)
                {
                    return;
                }
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException("Non optional string was null."));
                return;
            }
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                this.Write(
                    writer,
                    item,
                    nullTerminate: nullTerminate);
            }
        }

        public void Write(
            MutagenWriter writer,
            string item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask,
            bool nullTerminate = true)
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
                        errorMask,
                        nullTerminate: nullTerminate);
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
            IHasBeenSetItemGetter<string> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask,
            bool nullTerminate = true)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                header,
                fieldIndex,
                nullable,
                errorMask,
                nullTerminate: nullTerminate);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<string> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask,
            bool nullTerminate = true)
        {
            this.Write(
                writer,
                item.Item,
                header,
                fieldIndex,
                nullable,
                errorMask,
                nullTerminate: nullTerminate);
        }

        public void Write(MutagenWriter writer, string item, long length, ErrorMaskBuilder errorMask)
        {
            if (length != item.Length)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"Expected length was {item.Length}, but was passed {length}."));
            }
            Write(writer, item);
        }
    }
}
