using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Translations.Binary;

public sealed class P2Int16BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<P2Int16, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public static readonly P2Int16BinaryTranslation<TReader, TWriter> Instance = new();
    public override int ExpectedLength => 4;

    public override P2Int16 Parse(TReader reader)
    {
        return new P2Int16(
            reader.ReadInt16(),
            reader.ReadInt16());
    }

    public override void Write(TWriter writer, P2Int16 item)
    {
        writer.Write(item.X);
        writer.Write(item.Y);
    }

    public void Write(TWriter writer, P2Int16 item, bool swapCoords)
    {
        if (swapCoords)
        {
            writer.Write(item.Y);
            writer.Write(item.X);
        }
        else
        {
            Write(writer, item);
        }
    }

    public P2Int16 Read(ReadOnlySpan<byte> span)
    {
        return new P2Int16(
            x: BinaryPrimitives.ReadInt16LittleEndian(span),
            y: BinaryPrimitives.ReadInt16LittleEndian(span.Slice(2)));
    }

    public P2Int16 Read(ReadOnlySpan<byte> span, bool swapCoords)
    {
        if (swapCoords)
        {
            return new P2Int16(
                x: BinaryPrimitives.ReadInt16LittleEndian(span.Slice(2)),
                y: BinaryPrimitives.ReadInt16LittleEndian(span));
        }
        else
        {
            return Read(span);
        }
    }

    public P2Int16 Parse(TReader reader, bool swapCoords)
    {
        if (swapCoords)
        {
            return new P2Int16(
                y: reader.ReadInt16(),
                x: reader.ReadInt16());
        }
        return Parse(reader);
    }
}