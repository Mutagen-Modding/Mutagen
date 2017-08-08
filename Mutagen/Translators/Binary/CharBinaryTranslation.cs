using Noggog;
using System;

namespace Mutagen.Binary
{
    public class CharBinaryTranslation : PrimitiveBinaryTranslation<char>
    {
        public readonly static CharBinaryTranslation Instance = new CharBinaryTranslation();

        protected override char ParseBytes(byte[] bytes)
        {
            if (bytes.Length != 1)
            {
                throw new ArgumentException($"Length was not 1, and unexpected: {bytes.Length}");
            }
            return (char)bytes[0];
        }
    }
}
