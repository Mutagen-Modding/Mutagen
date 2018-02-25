using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class P3UInt16BinaryTranslation : PrimitiveBinaryTranslation<P3UInt16>
    {
        public readonly static P3UInt16BinaryTranslation Instance = new P3UInt16BinaryTranslation();
        public override ContentLength? ExpectedLength => new ContentLength(1);

        protected override P3UInt16 ParseValue(MutagenFrame reader)
        {
            return new P3UInt16(
                reader.Reader.ReadUInt16(),
                reader.Reader.ReadUInt16(),
                reader.Reader.ReadUInt16());
        }

        protected override void WriteValue(MutagenWriter writer, P3UInt16 item)
        {
            writer.Write(item.X);
            writer.Write(item.Y);
            writer.Write(item.Z);
        }
    }
}
