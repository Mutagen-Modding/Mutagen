using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public struct HeaderExport : IDisposable
    {
        public readonly MutagenWriter Writer;
        public readonly long SizePosition;
        public readonly sbyte MetaLengthToSkip;
        public readonly ObjectType Type;
        private static readonly byte[] SubRecordZeros = new byte[Constants.SUBRECORD_LENGTHLENGTH];
        private static readonly byte[] RecordZeros = new byte[Constants.RECORD_LENGTHLENGTH];

        private HeaderExport(
            MutagenWriter writer,
            long sizePosition,
            ObjectType type,
            sbyte metaLenToSkip)
        {
            this.Writer = writer;
            this.Type = type;
            this.SizePosition = sizePosition;
            this.MetaLengthToSkip = metaLenToSkip;
        }

        public static HeaderExport ExportHeader(
            MutagenWriter writer,
            RecordType record,
            ObjectType type)
        {
            writer.Write(record.TypeInt);
            var sizePosition = writer.Position;
            var offset = type.GetOffset(writer.GameMode);
            sbyte metaLen;
            switch (type)
            {
                case ObjectType.Subrecord:
                    writer.Write(SubRecordZeros);
                    metaLen = (sbyte)(Constants.SUBRECORD_LENGTHLENGTH + offset);
                    break;
                case ObjectType.Record:
                case ObjectType.Group:
                    writer.Write(RecordZeros);
                    metaLen = (SByte)(Constants.RECORD_LENGTHLENGTH + offset);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return new HeaderExport(writer, sizePosition, type, metaLen);
        }

        public static HeaderExport ExportSubRecordHeader(
            MutagenWriter writer,
            RecordType record)
        {
            return ExportHeader(writer, record, ObjectType.Subrecord);
        }

        public void Dispose()
        {
            var endPos = this.Writer.Position;
            this.Writer.Position = this.SizePosition;
            var diff = endPos - this.Writer.Position;
            var totalLength = diff - this.MetaLengthToSkip;
            switch (this.Type)
            {
                case ObjectType.Subrecord:
                    this.Writer.Write((Int16)totalLength);
                    break;
                case ObjectType.Record:
                case ObjectType.Group:
                    this.Writer.Write((Int32)totalLength);
                    break;
                default:
                    throw new NotImplementedException();
            }
            this.Writer.Position = endPos;
        }
    }
}
