using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class Int32BinaryTranslation : PrimitiveBinaryTranslation<int>
    {
        public readonly static Int32BinaryTranslation Instance = new Int32BinaryTranslation();
        public override byte? ExpectedLength => 4;

        protected override int ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadInt32();
        }

        protected override void WriteValue(MutagenWriter writer, int item)
        {
            writer.Write(item);
        }
    }
}
