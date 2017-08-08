using Noggog;
using System;

namespace Mutagen.Binary
{
    public class UInt32BinaryTranslation : PrimitiveBinaryTranslation<uint>
    {
        public readonly static UInt32BinaryTranslation Instance = new UInt32BinaryTranslation();

        protected override uint ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
