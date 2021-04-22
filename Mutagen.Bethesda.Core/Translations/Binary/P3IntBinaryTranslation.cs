using Noggog;
using System;
using System.Buffers.Binary;
using System.IO;

namespace Mutagen.Bethesda.Translations.Binary
{
    public class P3IntBinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<P3Int, TReader, TWriter>
        where TReader : IBinaryReadStream
        where TWriter : IBinaryWriteStream
    {
        public readonly static P3IntBinaryTranslation<TReader, TWriter> Instance = new();
        public override int ExpectedLength => 12;

        public override P3Int Parse(TReader reader)
        {
            return new P3Int(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32());
        }

        public override void Write(TWriter writer, P3Int item)
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
