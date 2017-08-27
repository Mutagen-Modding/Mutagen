using Noggog;
using System;

namespace Mutagen.Binary
{
    public class UInt64BinaryTranslation : PrimitiveBinaryTranslation<ulong>
    {
        public readonly static UInt64BinaryTranslation Instance = new UInt64BinaryTranslation();
        public override byte ExpectedLength => 8;

        protected override ulong ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
