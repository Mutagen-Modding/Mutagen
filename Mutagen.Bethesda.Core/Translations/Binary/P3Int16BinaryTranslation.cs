using Noggog;
using System;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Translations.Binary;

public class P3Int16BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<P3Int16, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public readonly static P3Int16BinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 4;

    public override P3Int16 Parse(TReader reader)
    {
        return new P3Int16(
            reader.ReadInt16(),
            reader.ReadInt16(),
            reader.ReadInt16());
    }

    public override void Write(TWriter writer, P3Int16 item)
    {
        writer.Write(item.X);
        writer.Write(item.Y);
        writer.Write(item.Z);
    }

    public P3Int16 Read(ReadOnlySpan<byte> span)
    {
        return new P3Int16(
            BinaryPrimitives.ReadInt16LittleEndian(span),
            BinaryPrimitives.ReadInt16LittleEndian(span.Slice(2)),
            BinaryPrimitives.ReadInt16LittleEndian(span.Slice(4)));
    }
}