using Noggog;
using System;

namespace Mutagen.Binary
{
    public class UInt16BinaryTranslation : PrimitiveBinaryTranslation<ushort>
    {
        public readonly static UInt16BinaryTranslation Instance = new UInt16BinaryTranslation();

        protected override ushort ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
