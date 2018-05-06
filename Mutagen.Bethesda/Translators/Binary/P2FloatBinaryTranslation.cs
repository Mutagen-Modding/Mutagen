using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class P2FloatBinaryTranslation : PrimitiveBinaryTranslation<P2Float>
    {
        public readonly static P2FloatBinaryTranslation Instance = new P2FloatBinaryTranslation();
        public override int? ExpectedLength => 1;

        protected override P2Float ParseValue(MutagenFrame reader)
        {
            return new P2Float(
                reader.Reader.ReadFloat(),
                reader.Reader.ReadFloat());
        }

        protected override void WriteValue(MutagenWriter writer, P2Float item)
        {
            writer.Write(item.X);
            writer.Write(item.Y);
        }
    }
}
