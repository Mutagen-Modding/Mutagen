using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class Int8BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<sbyte, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly Int8BinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 1;

    public override sbyte Parse(TReader reader)
    {
        return reader.ReadInt8();
    }

    public override void Write(TWriter writer, sbyte item)
    {
        writer.Write(item);
    }
}