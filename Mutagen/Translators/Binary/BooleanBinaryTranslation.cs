using Noggog;
using System;

namespace Mutagen.Binary
{
    public class BooleanBinaryTranslation : PrimitiveBinaryTranslation<bool>
    {
        public readonly static BooleanBinaryTranslation Instance = new BooleanBinaryTranslation();

        protected override bool ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
