using Noggog;
using System;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Translations.Binary;

public class BooleanBinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<bool, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : MutagenWriter
{
    public readonly static BooleanBinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 1;

    public override bool Parse(TReader reader)
    {
        return reader.ReadBoolean();
    }

    public override void Write(TWriter writer, bool item)
    {
        writer.Write(item);
    }

    public void Write(TWriter writer, bool item, byte byteLength)
    {
        writer.Write(item);
        writer.WriteZeros(byteLength);
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
