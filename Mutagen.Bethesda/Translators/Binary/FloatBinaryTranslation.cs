using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class FloatBinaryTranslation : PrimitiveBinaryTranslation<float>
    {
        public readonly static FloatBinaryTranslation Instance = new FloatBinaryTranslation();
        public override int? ExpectedLength => 4;

        protected override float ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadFloat();
        }

        protected override void WriteValue(MutagenWriter writer, float item)
        {
            writer.Write(item);
        }
    }
}
