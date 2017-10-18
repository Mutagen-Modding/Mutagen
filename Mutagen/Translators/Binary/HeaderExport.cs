using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Binary
{
    public struct HeaderExport : IDisposable
    {
        public readonly MutagenWriter Writer;
        public readonly long SizePosition;
        public readonly ObjectType Type;
        private const byte ZeroByte = 0;

        private HeaderExport(
            MutagenWriter writer,
            RecordType record,
            ObjectType type)
        {
            this.Writer = writer;
            this.Type = type;
            writer.Write(record.Type.ToCharArray());
            this.SizePosition = writer.Position;
            for (int i = 0; i < this.Type.GetLengthLength(); i++)
            {
                writer.Write(ZeroByte);
            }
        }

        public static HeaderExport ExportHeader(
            MutagenWriter writer,
            RecordType record,
            ObjectType type)
        {
            return new HeaderExport(writer, record, type);
        }

        public static HeaderExport ExportRecordHeader(
            MutagenWriter writer,
            RecordType record)
        {
            return new HeaderExport(writer, record, ObjectType.Record);
        }

        public static HeaderExport ExportSubRecordHeader(
            MutagenWriter writer,
            RecordType record)
        {
            return new HeaderExport(writer, record, ObjectType.Subrecord);
        }

        public void Dispose()
        {
            var endPos = this.Writer.Position;
            this.Writer.Position = this.SizePosition;
            var lengthLength = this.Type.GetLengthLength();
            var offset = this.Type.GetOffset();
            var diff = endPos - this.Writer.Position;
            var totalLength = diff - offset - lengthLength;
            switch (lengthLength)
            {
                case 2:
                    this.Writer.Write((Int16)totalLength);
                    break;
                case 4:
                    this.Writer.Write((Int32)totalLength);
                    break;
                default:
                    throw new NotImplementedException();
            }
            this.Writer.Position = endPos;
        }
    }
}
