using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class P3FloatBinaryTranslation : PrimitiveBinaryTranslation<P3Float>
    {
        public readonly static P3FloatBinaryTranslation Instance = new P3FloatBinaryTranslation();
        public override ContentLength? ExpectedLength => new ContentLength(1);

        protected override P3Float ParseValue(MutagenFrame reader)
        {
            return new P3Float(
                reader.Reader.ReadFloat(),
                reader.Reader.ReadFloat(),
                reader.Reader.ReadFloat());
        }

        protected override void WriteValue(MutagenWriter writer, P3Float item)
        {
            writer.Write(item.X);
            writer.Write(item.Y);
            writer.Write(item.Z);
        }
    }
}
