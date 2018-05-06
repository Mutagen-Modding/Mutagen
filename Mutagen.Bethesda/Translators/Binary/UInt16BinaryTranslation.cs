using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class UInt16BinaryTranslation : PrimitiveBinaryTranslation<ushort>
    {
        public readonly static UInt16BinaryTranslation Instance = new UInt16BinaryTranslation();
        public override int? ExpectedLength => 2;

        protected override ushort ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadUInt16();
        }

        protected override void WriteValue(MutagenWriter writer, ushort item)
        {
            writer.Write(item);
        }
    }
}
