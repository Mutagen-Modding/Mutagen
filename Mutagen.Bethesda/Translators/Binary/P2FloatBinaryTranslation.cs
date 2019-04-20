using Noggog;
using System;
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

        public override void WriteValue(MutagenWriter writer, P2Float item)
        {
            FloatBinaryTranslation.Instance.WriteValue(writer, item.X);
            FloatBinaryTranslation.Instance.WriteValue(writer, item.Y);
        }
    }
}
