using Noggog;
using System;

namespace Mutagen.Bethesda.Translations.Binary;

public class P3FloatBinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<P3Float, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public readonly static P3FloatBinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 12;

    public override P3Float Parse(TReader reader)
    {
        return new P3Float(
            FloatBinaryTranslation<TReader, TWriter>.Instance.Parse(reader),
            FloatBinaryTranslation<TReader, TWriter>.Instance.Parse(reader),
            FloatBinaryTranslation<TReader, TWriter>.Instance.Parse(reader));
    }

    public override void Write(TWriter writer, P3Float item)
    {
        FloatBinaryTranslation<TReader, TWriter>.Instance.Write(writer, item.X);
        FloatBinaryTranslation<TReader, TWriter>.Instance.Write(writer, item.Y);
        FloatBinaryTranslation<TReader, TWriter>.Instance.Write(writer, item.Z);
    }

    public P3Float Read(ReadOnlySpan<byte> span)
    {
        return new P3Float(
            span.Float(),
            span.Slice(4).Float(),
            span.Slice(8).Float());
    }
}