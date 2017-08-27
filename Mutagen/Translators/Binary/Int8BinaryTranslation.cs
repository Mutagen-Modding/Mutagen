using Noggog;
using System;

namespace Mutagen.Binary
{
    public class Int8BinaryTranslation : PrimitiveBinaryTranslation<sbyte>
    {
        public readonly static Int8BinaryTranslation Instance = new Int8BinaryTranslation();
        public override byte ExpectedLength => 1;

        protected override sbyte ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
