using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class StringBinaryTranslation : IBinaryTranslation<string, Exception>
    {
        public readonly static StringBinaryTranslation Instance = new StringBinaryTranslation();

        public TryGet<string> Parse(BinaryReader reader, bool doMasks, out Exception errorMask)
        {
            errorMask = null;
            throw new NotImplementedException();
        }

        public void Write(BinaryWriter writer, string item, bool doMasks, out Exception errorMask)
        {
            try
            {
                throw new NotImplementedException();
                errorMask = null;
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = ex;
            }
        }
    }
}
