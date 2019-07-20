using Noggog;
using System;
using System.Buffers.Binary;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class P2Int16BinaryTranslation : PrimitiveBinaryTranslation<P2Int16>
    {
        public readonly static P2Int16BinaryTranslation Instance = new P2Int16BinaryTranslation();
        public override int ExpectedLength => 4;

        public override P2Int16 ParseValue(MutagenFrame reader)
        {
            return new P2Int16(
                reader.Reader.ReadInt16(),
                reader.Reader.ReadInt16());
        }

        public override void WriteValue(MutagenWriter writer, P2Int16 item)
        {
            writer.Write(item.X);
            writer.Write(item.Y);
        }

        public static P2Int16 Read(ReadOnlySpan<byte> span)
        {
            return new P2Int16(
                BinaryPrimitives.ReadInt16LittleEndian(span),
                BinaryPrimitives.ReadInt16LittleEndian(span.Slice(2)));
        }
    }
}
