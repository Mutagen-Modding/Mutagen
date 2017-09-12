using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class FloatBinaryTranslation : PrimitiveBinaryTranslation<float>
    {
        public readonly static FloatBinaryTranslation Instance = new FloatBinaryTranslation();
        public override byte ExpectedLength => 4;

        protected override float ParseValue(BinaryReader reader)
        {
            return reader.ReadSingle();
        }

        protected override void WriteValue(BinaryWriter writer, float item)
        {
            writer.Write(item);
        }
    }
}
