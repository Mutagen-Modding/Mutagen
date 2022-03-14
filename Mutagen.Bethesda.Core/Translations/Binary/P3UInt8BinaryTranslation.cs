using Noggog;
using System;
using System.Buffers.Binary;
using System.IO;

namespace Mutagen.Bethesda.Translations.Binary;

public class P3UInt8BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<P3UInt8, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly P3UInt8BinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 6;

    public override P3UInt8 Parse(TReader reader)
    {
        return new P3UInt8(
            reader.ReadUInt8(),
            reader.ReadUInt8(),
            reader.ReadUInt8());
    }

    public override void Write(TWriter writer, P3UInt8 item)
    {
        writer.Write(item.X);
        writer.Write(item.Y);
        writer.Write(item.Z);
    }

    public P3UInt8 Read(ReadOnlySpan<byte> span)
    {
        return new P3UInt8(
            span[0],
            span[1],
            span[2]);
    }
}