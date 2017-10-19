using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class BooleanBinaryTranslation : PrimitiveBinaryTranslation<bool>
    {
        public readonly static BooleanBinaryTranslation Instance = new BooleanBinaryTranslation();
        public override byte? ExpectedLength => 1;

        protected override bool ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadBoolean();
        }

        protected override void WriteValue(MutagenWriter writer, bool item)
        {
            writer.Write(item);
        }
    }
}
