using Noggog;
using System;

namespace Mutagen.Binary
{
    public class UInt64BinaryTranslation : PrimitiveBinaryTranslation<ulong>
    {
        public readonly static UInt64BinaryTranslation Instance = new UInt64BinaryTranslation();

        protected override ulong ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
