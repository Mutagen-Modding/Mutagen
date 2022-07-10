using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class P3UInt16BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<P3UInt16, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly P3UInt16BinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 6;

    public override P3UInt16 Parse(TReader reader)
    {
        return new P3UInt16(
            reader.ReadUInt16(),
            reader.ReadUInt16(),
            reader.ReadUInt16());
    }

    public override void Write(TWriter writer, P3UInt16 item)
    {
        writer.Write(item.X);
        writer.Write(item.Y);
        writer.Write(item.Z);
    }

    public P3UInt16 Read(ReadOnlySpan<byte> span)
    {
        return new P3UInt16(
            BinaryPrimitives.ReadUInt16LittleEndian(span),
            BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(2)),
            BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(4)));
    }
}