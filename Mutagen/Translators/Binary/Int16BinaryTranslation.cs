using Noggog;
using System;

namespace Mutagen.Binary
{
    public class Int16BinaryTranslation : PrimitiveBinaryTranslation<short>
    {
        public readonly static Int16BinaryTranslation Instance = new Int16BinaryTranslation();

        protected override short ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
