using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class UInt32BinaryTranslation : PrimitiveBinaryTranslation<uint>
    {
        public readonly static UInt32BinaryTranslation Instance = new UInt32BinaryTranslation();
        public override int? ExpectedLength => 4;

        protected override uint ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadUInt32();
        }

        protected override void WriteValue(MutagenWriter writer, uint item)
        {
            writer.Write(item);
        }
    }
}
