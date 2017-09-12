using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class Int8BinaryTranslation : PrimitiveBinaryTranslation<sbyte>
    {
        public readonly static Int8BinaryTranslation Instance = new Int8BinaryTranslation();
        public override byte ExpectedLength => 1;

        protected override sbyte ParseValue(BinaryReader reader)
        {
            return reader.ReadSByte();
        }

        protected override void WriteValue(BinaryWriter writer, sbyte item)
        {
            writer.Write(item);
        }
    }
}
