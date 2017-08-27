using Noggog;
using System;

namespace Mutagen.Binary
{
    public class Int64BinaryTranslation : PrimitiveBinaryTranslation<long>
    {
        public readonly static Int64BinaryTranslation Instance = new Int64BinaryTranslation();
        public override byte ExpectedLength => 8;

        protected override long ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
