using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class DoubleBinaryTranslation : PrimitiveBinaryTranslation<double>
    {
        public readonly static DoubleBinaryTranslation Instance = new DoubleBinaryTranslation();
        public override int? ExpectedLength => 4;

        public override double ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadDouble();
        }

        public override void WriteValue(MutagenWriter writer, double item)
        {
            writer.Write(item);
        }
    }
}
