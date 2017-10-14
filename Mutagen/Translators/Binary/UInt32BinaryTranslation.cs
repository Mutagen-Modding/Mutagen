using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class UInt32BinaryTranslation : PrimitiveBinaryTranslation<uint>
    {
        public readonly static UInt32BinaryTranslation Instance = new UInt32BinaryTranslation();
        public override byte? ExpectedLength => 4;
        
        protected override uint ParseValue(BinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        protected override void WriteValue(BinaryWriter writer, uint item)
        {
            writer.Write(item);
        }
    }
}
