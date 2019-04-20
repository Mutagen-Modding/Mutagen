using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class Int8BinaryTranslation : PrimitiveBinaryTranslation<sbyte>
    {
        public readonly static Int8BinaryTranslation Instance = new Int8BinaryTranslation();
        public override int ExpectedLength => 1;

        public override sbyte ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadInt8();
        }

        public override void WriteValue(MutagenWriter writer, sbyte item)
        {
            writer.Write(item);
        }
    }
}
