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

        public virtual bool Parse(
            MutagenFrame frame,
            out string item)
        {
            return Parse(
                frame: frame,
                parseWhole: true,
                item: out item);
        }

        public bool Parse(
            MutagenFrame frame,
            bool parseWhole,
            out string item)
        {
            item = Parse(frame, parseWhole: parseWhole);
            return true;
        }

        public virtual string Parse(
            MutagenFrame frame,
            bool parseWhole = true,
            StringBinaryType stringBinaryType = StringBinaryType.NullTerminate)
        {
            switch (stringBinaryType)
            {
                case StringBinaryType.Plain:
                case StringBinaryType.NullTerminate:
                    if (parseWhole)
                    {
                        return BinaryStringUtility.ProcessWholeToZString(frame.ReadMemory(checked((int)frame.Remaining)));
                    }
                    else
                    {
                        return BinaryStringUtility.ParseUnknownLengthString(frame.Reader);
                    }
                case StringBinaryType.PrependLength:
                    var len = frame.ReadInt32();
                    return BinaryStringUtility.ToZString(frame.ReadMemory(len));
                default:
                    throw new NotImplementedException();
            }
        }

        public void Write(
            MutagenWriter writer,
            string item)
        {
            writer.Write(item, binaryType: StringBinaryType.NullTerminate);
        }

        public void WriteNullable(
            MutagenWriter writer,
            string? item)
        {
            if (item == null) return;
            writer.Write(item, binaryType: StringBinaryType.NullTerminate);
        }

        public void Write(
            MutagenWriter writer,
            string item,
            StringBinaryType binaryType)
        {
            writer.Write(item, binaryType);
        }

        public void Write(
            MutagenWriter writer,
            string item,
            RecordType header,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                writer.Write(
                    item,
                    binaryType: binaryType);
            }
        }

        public void WriteNullable(
            MutagenWriter writer,
            string? item,
            RecordType header,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            if (item == null) return;
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                writer.Write(
                    item,
                    binaryType: binaryType);
            }
        }

        public void WriteNullable(
            MutagenWriter writer,
            string? item,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            if (item == null) return;
            writer.Write(
                item,
                binaryType: binaryType);
        }

        public void Write(MutagenWriter writer, string item, long length)
        {
            if (length != item.Length)
            {
                throw new ArgumentException($"Expected length was {item.Length}, but was passed {length}.");
            }
            writer.Write(item, binaryType: StringBinaryType.NullTerminate);
        }
    }
}
