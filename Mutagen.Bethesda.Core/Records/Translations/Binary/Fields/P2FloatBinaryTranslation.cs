using Noggog;
using System;
using System.Buffers.Binary;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class P2FloatBinaryTranslation : PrimitiveBinaryTranslation<P2Float>
    {
        public readonly static P2FloatBinaryTranslation Instance = new P2FloatBinaryTranslation();
        public override int ExpectedLength => 8;

        public override P2Float ParseValue(MutagenFrame reader)
        {
            return new P2Float(
                FloatBinaryTranslation.Instance.ParseValue(reader),
                FloatBinaryTranslation.Instance.ParseValue(reader));
        }

        public override void Write(MutagenWriter writer, P2Float item)
        {
            FloatBinaryTranslation.Instance.Write(writer, item.X);
            FloatBinaryTranslation.Instance.Write(writer, item.Y);
        }

        public static P2Float Read(ReadOnlySpan<byte> span)
        {
            return new P2Float(
                span.Float(),
                span.Slice(4).Float());
        }
    }
}
