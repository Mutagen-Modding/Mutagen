using Noggog;
using System;

namespace Mutagen.Bethesda.Translations.Binary;

public class DoubleBinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<double, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public readonly static DoubleBinaryTranslation<TReader, TWriter> Instance = new();
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