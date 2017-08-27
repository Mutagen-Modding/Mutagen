using Noggog;
using System;

namespace Mutagen.Binary
{
    public class BooleanBinaryTranslation : PrimitiveBinaryTranslation<bool>
    {
        public readonly static BooleanBinaryTranslation Instance = new BooleanBinaryTranslation();
        public override byte ExpectedLength => 1;

        protected override bool ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
