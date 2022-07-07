using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class P2IntBinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<P2Int, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly P2IntBinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 8;

    public override P2Int Parse(TReader reader)
    {
        return new P2Int(
            reader.ReadInt32(),
            reader.ReadInt32());
    }

    public override void Write(TWriter writer, P2Int item)
    {
        writer.Write(item.X);
        writer.Write(item.Y);
    }

    public P2Int Read(ReadOnlySpan<byte> span)
    {
        return new P2Int(
            BinaryPrimitives.ReadInt32LittleEndian(span),
            BinaryPrimitives.ReadInt32LittleEndian(span.Slice(4)));
    }
}