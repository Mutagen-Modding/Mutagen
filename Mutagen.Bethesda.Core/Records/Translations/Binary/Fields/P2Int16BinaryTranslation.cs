using Mutagen.Bethesda.Records.Binary.Streams;
using Noggog;
using System;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Binary
{
    public class P2Int16BinaryTranslation : PrimitiveBinaryTranslation<P2Int16>
    {
        public readonly static P2Int16BinaryTranslation Instance = new P2Int16BinaryTranslation();
        public override int ExpectedLength => 4;

        public override P2Int16 ParseValue(MutagenFrame reader)
        {
            return new P2Int16(
                reader.Reader.ReadInt16(),
                reader.Reader.ReadInt16());
        }

        public override void Write(MutagenWriter writer, P2Int16 item)
        {
            writer.Write(item.X);
            writer.Write(item.Y);
        }

        public void Write(MutagenWriter writer, P2Int16 item, bool swapCoords)
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

        public static P2Int16 Read(ReadOnlySpan<byte> span)
        {
            return new P2Int16(
                x: BinaryPrimitives.ReadInt16LittleEndian(span),
                y: BinaryPrimitives.ReadInt16LittleEndian(span.Slice(2)));
        }

        public static P2Int16 Read(ReadOnlySpan<byte> span, bool swapCoords)
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

        public P2Int16 Parse(MutagenFrame frame,
            bool swapCoords,
            P2Int16 defaultVal = default)
        {
            if (swapCoords)
            {
                return new P2Int16(
                    y: frame.Reader.ReadInt16(),
                    x: frame.Reader.ReadInt16());
            }
            return Parse(frame, defaultVal);
        }
    }
}
