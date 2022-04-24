using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public class ByteArrayBinaryTranslation<TReader, TWriter> : TypicalBinaryTranslation<MemorySlice<byte>, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public readonly static ByteArrayBinaryTranslation<TReader, TWriter> Instance = new();

    public override void Write(TWriter writer, MemorySlice<byte> item)
    {
        writer.Write(item);
    }

    public override MemorySlice<byte> Parse(TReader reader)
    {
        return reader.ReadBytes(checked((int)reader.Remaining));
    }

    protected override MemorySlice<byte> ParseBytes(MemorySlice<byte> bytes)
    {
        return bytes;
    }

    public void Write(TWriter writer, ReadOnlySpan<byte> item)
    {
        writer.Write(item);
    }

    public void Write(TWriter writer, ReadOnlyMemorySlice<byte> item)
    {
        writer.Write(item.Span);
    }
}