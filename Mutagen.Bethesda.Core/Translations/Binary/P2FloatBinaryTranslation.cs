using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Translations.Binary;

public class P2FloatBinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<P2Float, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public readonly static P2FloatBinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 8;

    public override void Write(TWriter writer, P2Float item)
    {
        FloatBinaryTranslation<TReader, TWriter>.Instance.Write(writer, item.X);
        FloatBinaryTranslation<TReader, TWriter>.Instance.Write(writer, item.Y);
    }

    public P2Float Read(ReadOnlySpan<byte> span)
    {
        return new P2Float(
            span.Float(),
            span[4..].Float());
    }

    public override P2Float Parse(TReader reader)
    {
        return Read(reader.ReadSpan(ExpectedLength));
    }
}