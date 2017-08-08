using Noggog;
using System;

namespace Mutagen.Binary
{
    public class ByteBinaryTranslation : PrimitiveBinaryTranslation<byte>
    {
        public readonly static ByteBinaryTranslation Instance = new ByteBinaryTranslation();

        protected override byte ParseBytes(byte[] bytes)
        {
            if (bytes.Length != 1)
            {
                throw new ArgumentException($"Length was not 1, and unexpected: {bytes.Length}");
            }
            return bytes[0];
        }
    }
}
