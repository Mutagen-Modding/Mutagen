using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class ByteBinaryTranslation : PrimitiveBinaryTranslation<byte>
    {
        public readonly static ByteBinaryTranslation Instance = new ByteBinaryTranslation();
        public override int? ExpectedLength => 1;

        public override byte ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadUInt8();
        }

        public override void WriteValue(MutagenWriter writer, byte item)
        {
            writer.Write(item);
        }
    }
}
