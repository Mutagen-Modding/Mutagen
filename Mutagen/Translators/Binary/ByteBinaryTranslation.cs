using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class ByteBinaryTranslation : PrimitiveBinaryTranslation<byte>
    {
        public readonly static ByteBinaryTranslation Instance = new ByteBinaryTranslation();
        public override byte ExpectedLength => 1;
        
        protected override byte ParseValue(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        protected override void WriteValue(BinaryWriter writer, byte item)
        {
            writer.Write(item);
        }
    }
}
