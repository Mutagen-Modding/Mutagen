using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class UInt64BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<ulong, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly UInt64BinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 8;

    public override ulong Parse(TReader reader)
    {
        return reader.ReadUInt64();
    }

    public override void Write(TWriter writer, ulong item)
    {
        writer.Write(item);
    }
}