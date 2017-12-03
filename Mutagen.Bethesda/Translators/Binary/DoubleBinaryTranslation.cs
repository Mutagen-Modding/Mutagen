using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class DoubleBinaryTranslation : PrimitiveBinaryTranslation<double>
    {
        public readonly static DoubleBinaryTranslation Instance = new DoubleBinaryTranslation();
        public override ContentLength? ExpectedLength => new ContentLength(4);

        protected override double ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadDouble();
        }

        protected override void WriteValue(MutagenWriter writer, double item)
        {
            writer.Write(item);
        }
    }
}
