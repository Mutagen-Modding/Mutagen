using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class Int16BinaryTranslation : PrimitiveBinaryTranslation<short>
    {
        public readonly static Int16BinaryTranslation Instance = new Int16BinaryTranslation();
        public override int? ExpectedLength => 2;

        protected override short ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadInt16();
        }

        protected override void WriteValue(MutagenWriter writer, short item)
        {
            writer.Write(item);
        }
    }
}
