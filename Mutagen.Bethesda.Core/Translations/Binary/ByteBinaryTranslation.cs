using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class ByteBinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<byte, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly ByteBinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 1;

    public override byte Parse(TReader reader)
    {
        return reader.ReadUInt8();
    }

    public override void Write(TWriter writer, byte item)
    {
        writer.Write(item);
    }
}