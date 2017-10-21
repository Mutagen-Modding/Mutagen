using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class ByteBinaryTranslation : PrimitiveBinaryTranslation<byte>
    {
        public readonly static ByteBinaryTranslation Instance = new ByteBinaryTranslation();
        public override ContentLength? ExpectedLength => new ContentLength(1);

        protected override byte ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadByte();
        }

        protected override void WriteValue(MutagenWriter writer, byte item)
        {
            writer.Write(item);
        }
    }
}
