using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class BooleanBinaryTranslation : PrimitiveBinaryTranslation<bool>
    {
        public readonly static BooleanBinaryTranslation Instance = new BooleanBinaryTranslation();
        public override int ExpectedLength => 1;

        public override bool ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadBool();
        }

        public override void Write(MutagenWriter writer, bool item)
        {
            writer.Write(item);
        }

        public void WriteAsMarker(
            MutagenWriter writer,
            bool item,
            RecordType header)
        {
            if (!item) return;
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                // Presence of marker signifies true
            }
        }

        public bool Parse(
            MutagenFrame frame,
            byte byteLength)
        {
            return byteLength switch
            {
                1 => frame.ReadBool(),
                2 => frame.ReadUInt16() > 0,
                4 => frame.ReadUInt32() > 0,
                _ => throw new NotImplementedException(),
            };
        }

        public void Write(
            MutagenWriter writer,
            bool item,
            RecordType header,
            byte byteLength)
        {
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                writer.Write(item ? 1 : 0, byteLength);
            }
        }

        public void WriteNullable(
            MutagenWriter writer,
            bool? item,
            RecordType header,
            byte byteLength)
        {
            if (!item.HasValue) return;
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                writer.Write(item.Value ? 1 : 0, byteLength);
            }
        }
    }
}
