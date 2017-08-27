using Noggog;
using System;

namespace Mutagen.Binary
{
    public class Int32BinaryTranslation : PrimitiveBinaryTranslation<int>
    {
        public readonly static Int32BinaryTranslation Instance = new Int32BinaryTranslation();
        public override byte ExpectedLength => 4;

        protected override int ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
