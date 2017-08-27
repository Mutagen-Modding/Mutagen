using Noggog;
using System;

namespace Mutagen.Binary
{
    public class FloatBinaryTranslation : PrimitiveBinaryTranslation<float>
    {
        public readonly static FloatBinaryTranslation Instance = new FloatBinaryTranslation();
        public override byte ExpectedLength => 4;

        protected override float ParseBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
