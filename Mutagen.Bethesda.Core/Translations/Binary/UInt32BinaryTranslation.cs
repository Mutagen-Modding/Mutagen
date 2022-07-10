using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class UInt32BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<uint, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly UInt32BinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 4;

    public override uint Parse(TReader reader)
    {
        return reader.ReadUInt32();
    }

    public override void Write(TWriter writer, uint item)
    {
        writer.Write(item);
    }
}