using Noggog;
using System;

namespace Mutagen.Binary
{
    public class DoubleBinaryTranslation : PrimitiveBinaryTranslation<double>
    {
        public readonly static DoubleBinaryTranslation Instance = new DoubleBinaryTranslation();
        public override byte ExpectedLength => 4;

        protected override double ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
