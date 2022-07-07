using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class DoubleBinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<double, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly DoubleBinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 8;

    public override double Parse(TReader reader)
    {
        return reader.ReadDouble();
    }

    public override void Write(TWriter writer, double item)
    {
        writer.Write(item);
    }
}