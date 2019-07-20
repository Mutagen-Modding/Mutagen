using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
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

        public override void WriteValue(MutagenWriter writer, P3Float item)
        {
            FloatBinaryTranslation.Instance.WriteValue(writer, item.X);
            FloatBinaryTranslation.Instance.WriteValue(writer, item.Y);
            FloatBinaryTranslation.Instance.WriteValue(writer, item.Z);
        }

        public static P3Float Read(ReadOnlySpan<byte> span)
        {
            return new P3Float(
                SpanExt.GetFloat(span),
                SpanExt.GetFloat(span.Slice(4)),
                SpanExt.GetFloat(span.Slice(8)));
        }
    }
}
