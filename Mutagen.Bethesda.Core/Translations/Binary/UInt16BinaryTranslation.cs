using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Translations.Binary;

public class UInt16BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<ushort, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public readonly static UInt16BinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 2;

    public override ushort Parse(TReader reader)
    {
        return reader.ReadUInt16();
    }

    public override void Write(TWriter writer, ushort item)
    {
        writer.Write(item);
    }
}