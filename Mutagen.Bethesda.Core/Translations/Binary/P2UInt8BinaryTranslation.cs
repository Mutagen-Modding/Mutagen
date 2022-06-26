using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public class P2UInt8BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<P2UInt8, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly P2UInt8BinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 6;

    public override P2UInt8 Parse(TReader reader)
    {
        return new P2UInt8(
            reader.ReadUInt8(),
            reader.ReadUInt8());
    }

    public override void Write(TWriter writer, P2UInt8 item)
    {
        writer.Write(item.X);
        writer.Write(item.Y);
    }

    public P2UInt8 Read(ReadOnlySpan<byte> span)
    {
        return new P2UInt8(
            span[0],
            span[1]);
    }
}