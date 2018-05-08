using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class BooleanBinaryTranslation : PrimitiveBinaryTranslation<bool>
    {
        public readonly static BooleanBinaryTranslation Instance = new BooleanBinaryTranslation();
        public override int? ExpectedLength => 1;

        protected override bool ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadBool();
        }

        protected override void WriteValue(MutagenWriter writer, bool item)
        {
            writer.Write(item);
        }
    }
}
