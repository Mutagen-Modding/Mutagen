using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class Int64BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<long, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly Int64BinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 8;

    public override long Parse(TReader reader)
    {
        return reader.ReadInt64();
    }

    public override void Write(TWriter writer, long item)
    {
        writer.Write(item);
    }
}