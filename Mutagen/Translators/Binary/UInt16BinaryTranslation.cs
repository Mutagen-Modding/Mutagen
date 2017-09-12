using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class UInt16BinaryTranslation : PrimitiveBinaryTranslation<ushort>
    {
        public readonly static UInt16BinaryTranslation Instance = new UInt16BinaryTranslation();
        public override byte ExpectedLength => 2;

        protected override ushort ParseValue(BinaryReader reader)
        {
            return reader.ReadUInt16();
        }

        protected override void WriteValue(BinaryWriter writer, ushort item)
        {
            writer.Write(item);
        }
    }
}
