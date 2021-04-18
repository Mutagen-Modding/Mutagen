using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public class ByteArrayBinaryTranslation : TypicalBinaryTranslation<MemorySlice<byte>>
    {
        public readonly static ByteArrayBinaryTranslation Instance = new ByteArrayBinaryTranslation();

        public override void Write(MutagenWriter writer, MemorySlice<byte> item)
        {
            writer.Write(item);
        }

        protected override MemorySlice<byte> ParseValue(MutagenFrame frame)
        {
            return frame.Reader.ReadBytes(checked((int)frame.Remaining));
        }

        protected override MemorySlice<byte> ParseBytes(MemorySlice<byte> bytes)
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

        public void Write(
            MutagenWriter writer,
            ReadOnlyMemorySlice<byte>? item,
            RecordType header)
        {
            try
            {
                if (!item.HasValue) return;
                using (HeaderExport.Subrecord(writer, header))
                {
                    Write(writer, item.Value.Span);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, header);
            }
        }

        public void Write(
            MutagenWriter writer,
            ReadOnlyMemorySlice<byte>? item,
            RecordType header,
            RecordType overflowRecord)
        {
            try
            {
                if (!item.HasValue) return;
                if (item.Value.Length > ushort.MaxValue)
                {
                    using (HeaderExport.Subrecord(writer, overflowRecord))
                    {
                        writer.Write(item.Value.Length);
                    }
                    using (HeaderExport.Subrecord(writer, header))
                    {
                    }
                    Write(writer, item.Value.Span);
                }
                else
                {
                    using (HeaderExport.Subrecord(writer, header))
                    {
                        Write(writer, item.Value.Span);
                    }
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, header);
            }
        }
    }
}
