using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class Int16BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<short, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly Int16BinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 2;

    public override short Parse(TReader reader)
    {
        return reader.ReadInt16();
    }

    public override void Write(TWriter writer, short item)
    {
        writer.Write(item);
    }
}