using Mutagen.Bethesda.Records.Binary.Streams;
using Noggog;
using System;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public class P3FloatBinaryTranslation : PrimitiveBinaryTranslation<P3Float>
    {
        public readonly static P3FloatBinaryTranslation Instance = new P3FloatBinaryTranslation();
        public override int ExpectedLength => 12;

        public override P3Float ParseValue(MutagenFrame reader)
        {
            return new P3Float(
                FloatBinaryTranslation.Instance.ParseValue(reader),
                FloatBinaryTranslation.Instance.ParseValue(reader),
                FloatBinaryTranslation.Instance.ParseValue(reader));
        }

        public override void Write(MutagenWriter writer, P3Float item)
        {
            FloatBinaryTranslation.Instance.Write(writer, item.X);
            FloatBinaryTranslation.Instance.Write(writer, item.Y);
            FloatBinaryTranslation.Instance.Write(writer, item.Z);
        }

        public static P3Float Read(ReadOnlySpan<byte> span)
        {
            return new P3Float(
                span.Float(),
                span.Slice(4).Float(),
                span.Slice(8).Float());
        }
    }
}
