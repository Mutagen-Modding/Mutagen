using Mutagen.Bethesda.Records.Binary.Streams;
using Noggog;
using System;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Binary
{
    public class P3IntBinaryTranslation : PrimitiveBinaryTranslation<P3Int>
    {
        public readonly static P3IntBinaryTranslation Instance = new P3IntBinaryTranslation();
        public override int ExpectedLength => 12;

        public override P3Int ParseValue(MutagenFrame reader)
        {
            return new P3Int(
                reader.Reader.ReadInt32(),
                reader.Reader.ReadInt32(),
                reader.Reader.ReadInt32());
        }

        public override void Write(MutagenWriter writer, P3Int item)
        {
            writer.Write(item.X);
            writer.Write(item.Y);
            writer.Write(item.Z);
        }

        public static P3Int Read(ReadOnlySpan<byte> span)
        {
            return new P3Int(
                BinaryPrimitives.ReadInt32LittleEndian(span),
                BinaryPrimitives.ReadInt32LittleEndian(span.Slice(4)),
                BinaryPrimitives.ReadInt32LittleEndian(span.Slice(8)));
        }
    }
}
