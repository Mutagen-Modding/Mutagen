using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class Int32BinaryTranslation : PrimitiveBinaryTranslation<int>
    {
        public readonly static Int32BinaryTranslation Instance = new Int32BinaryTranslation();
        public override int ExpectedLength => 4;

        public override int ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadInt32();
        }

        public override void Write(MutagenWriter writer, int item)
        {
            writer.Write(item);
        }
    }
}
