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
            out string item,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            item = Parse(frame, parseWhole: parseWhole, stringBinaryType: binaryType);
            return true;
        }

        public string Parse(
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
                    {
                        var len = frame.ReadInt32();
                        return BinaryStringUtility.ToZString(frame.ReadMemory(len));
                    }
                case StringBinaryType.PrependLengthUShort:
                    {
                        var len = frame.ReadInt16();
                        return BinaryStringUtility.ToZString(frame.ReadMemory(len));
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public TranslatedString Parse(
            MutagenFrame frame,
            StringsSource source,
            StringBinaryType stringBinaryType,
            bool parseWhole = true)
        {
            if (frame.MetaData.StringsLookup != null)
            {
                if (frame.Remaining != 4)
                {
                    throw new ArgumentException($"String in Strings File format had unexpected length: {frame.Remaining} != 4");
                }
                uint key = frame.ReadUInt32();
                if (key == 0) return new TranslatedString(directString: null);
                return frame.MetaData.StringsLookup.CreateString(source, key);
            }
            else
            {
                return Parse(frame, parseWhole, stringBinaryType);
            }
        }

        public bool Parse(
            MutagenFrame frame,
            StringsSource source,
            StringBinaryType binaryType,
            out TranslatedString item,
            bool parseWhole = true)
        {
            item = Parse(frame, source, binaryType, parseWhole);
            return true;
        }

        public TranslatedString Parse(
            ReadOnlyMemorySlice<byte> data,
            StringsSource source,
            IStringsFolderLookup? lookup)
        {
            if (lookup != null)
            {
                if (data.Length != 4)
                {
                    throw new ArgumentException($"String in Strings File format had unexpected length: {data.Length} != 4");
                }
                uint key = BinaryPrimitives.ReadUInt32LittleEndian(data);
                if (key == 0) return new TranslatedString(directString: null);
                return lookup.CreateString(source, key);
            }
            else
            {
                return BinaryStringUtility.ProcessWholeToZString(data);
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
            try
            {
                using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
                {
                    writer.Write(
                        item,
                        binaryType: binaryType);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }

        public void WriteNullable(
            MutagenWriter writer,
            string? item,
            RecordType header,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            if (item == null) return;
            try
            {
                using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
                {
                    writer.Write(
                        item,
                        binaryType: binaryType);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }

        public void Write(
            MutagenWriter writer,
            ITranslatedStringGetter item,
            RecordType header,
            StringBinaryType binaryType,
            StringsSource source)
        {
            try
            {
                using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
                {
                    Write(
                        writer,
                        item,
                        binaryType,
                        source);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }

        public void Write(
            MutagenWriter writer,
            ITranslatedStringGetter item,
            StringBinaryType binaryType,
            StringsSource source)
        {
            if (writer.MetaData.StringsWriter == null)
            {
                writer.Write(
                    item.String ?? string.Empty,
                    binaryType: binaryType);
            }
            else
            {
                writer.Write(writer.MetaData.StringsWriter.Register(item, source));
            }
        }

        public void WriteNullable(
            MutagenWriter writer,
            ITranslatedStringGetter? item,
            RecordType header,
            StringBinaryType binaryType,
            StringsSource source)
        {
            if (item == null) return;
            try
            {
                using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
                {
                    if (writer.MetaData.StringsWriter == null)
                    {
                        writer.Write(
                            item.String ?? string.Empty,
                            binaryType: binaryType);
                    }
                    else
                    {
                        writer.Write(writer.MetaData.StringsWriter.Register(item, source));
                    }
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }

        public void WriteNullable(
            MutagenWriter writer,
            ITranslatedStringGetter? item,
            StringBinaryType binaryType,
            StringsSource source)
        {
            if (item == null) return;
            if (writer.MetaData.StringsWriter == null)
            {
                writer.Write(
                    item.String ?? string.Empty,
                    binaryType: binaryType);
            }
            else
            {
                writer.Write(writer.MetaData.StringsWriter.Register(item, source));
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
