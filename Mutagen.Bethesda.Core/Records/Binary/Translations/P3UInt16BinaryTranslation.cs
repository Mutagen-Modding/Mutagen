using Mutagen.Bethesda.Records.Binary.Streams;
using Noggog;
using System;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public class P3UInt16BinaryTranslation : PrimitiveBinaryTranslation<P3UInt16>
    {
        public readonly static P3UInt16BinaryTranslation Instance = new P3UInt16BinaryTranslation();
        public override int ExpectedLength => 6;

        public override P3UInt16 ParseValue(MutagenFrame reader)
        {
            return new P3UInt16(
                reader.Reader.ReadUInt16(),
                reader.Reader.ReadUInt16(),
                reader.Reader.ReadUInt16());
        }

        public override void Write(MutagenWriter writer, P3UInt16 item)
        {
            writer.Write(item.X);
            writer.Write(item.Y);
            writer.Write(item.Z);
        }

        public static P3UInt16 Read(ReadOnlySpan<byte> span)
        {
            return new P3UInt16(
                BinaryPrimitives.ReadUInt16LittleEndian(span),
                BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(2)),
                BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(4)));
        }
    }
}
