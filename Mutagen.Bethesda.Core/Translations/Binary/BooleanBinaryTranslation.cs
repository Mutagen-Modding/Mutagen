using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class BooleanBinaryTranslation<TReader> : PrimitiveBinaryTranslation<bool, TReader, MutagenWriter>
    where TReader : IBinaryReadStream
{
    public static readonly BooleanBinaryTranslation<TReader> Instance = new();
    public override int ExpectedLength => 1;

    public override bool Parse(TReader reader)
    {
        return reader.ReadBoolean();
    }

    public override void Write(MutagenWriter writer, bool item)
    {
        writer.Write(item);
    }

    public void Write(MutagenWriter writer, bool item, byte byteLength)
    {
        writer.Write(item, byteLength);
    }

    public bool Parse(
        TReader reader,
        byte byteLength)
    {
        return byteLength switch
        {
            1 => reader.ReadBoolean(),
            2 => reader.ReadUInt16() > 0,
            4 => reader.ReadUInt32() > 0,
            _ => throw new NotImplementedException(),
        };
    }

    public bool Parse(
        TReader reader,
        byte byteLength,
        byte importantByteLength)
    {
        var ret = Parse(reader, importantByteLength);
        reader.Position += byteLength - importantByteLength;
        return ret;
    }
}
