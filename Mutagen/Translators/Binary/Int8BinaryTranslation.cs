using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class Int8BinaryTranslation : PrimitiveBinaryTranslation<sbyte>
    {
        public readonly static Int8BinaryTranslation Instance = new Int8BinaryTranslation();
        public override ContentLength? ExpectedLength => new ContentLength(1);

        protected override sbyte ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadSByte();
        }

        protected override void WriteValue(MutagenWriter writer, sbyte item)
        {
            writer.Write(item);
        }
    }
}
