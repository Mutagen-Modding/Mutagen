using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class CharBinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<char, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly CharBinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 1;

    public override char Parse(TReader reader)
    {
        return (char)reader.ReadUInt8();
    }

    public override void Write(TWriter writer, char item)
    {
        writer.Write((byte)item);
    }
}