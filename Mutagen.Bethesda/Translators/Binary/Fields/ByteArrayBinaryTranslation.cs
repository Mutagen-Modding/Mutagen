using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public class ByteArrayBinaryTranslation : TypicalBinaryTranslation<byte[]>
    {
        public readonly static ByteArrayBinaryTranslation Instance = new ByteArrayBinaryTranslation();

        public override void Write(MutagenWriter writer, byte[] item)
        {
            writer.Write(item);
        }

        protected override Byte[] ParseValue(MutagenFrame frame)
        {
            return frame.Reader.ReadBytes(checked((int)frame.Remaining));
        }

        protected override byte[] ParseBytes(byte[] bytes)
        {
            return bytes;
        }

        public void Write(MutagenWriter writer, ReadOnlySpan<byte> item)
        {
            writer.Write(item);
        }

        public void Write(MutagenWriter writer, ReadOnlyMemorySlice<byte> item)
        {
            writer.Write(item.Span);
        }

        public void Write(MutagenWriter writer, ReadOnlyMemorySlice<byte>? item)
        {
            if (!item.HasValue) return;
            writer.Write(item.Value.Span);
        }

        public void Write(
            MutagenWriter writer,
            ReadOnlySpan<byte> item,
            RecordType header)
        {
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                Write(writer, item);
            }
        }

        public void Write(
            MutagenWriter writer,
            ReadOnlyMemorySlice<byte>? item,
            RecordType header)
        {
            if (!item.HasValue) return;
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                Write(writer, item.Value.Span);
            }
        }
    }
}
