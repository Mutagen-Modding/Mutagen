using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class DoubleBinaryTranslation : PrimitiveBinaryTranslation<double>
    {
        public readonly static DoubleBinaryTranslation Instance = new DoubleBinaryTranslation();
        public override byte ExpectedLength => 4;

        protected override double ParseValue(BinaryReader reader)
        {
            return reader.ReadDouble();
        }
    }
}
