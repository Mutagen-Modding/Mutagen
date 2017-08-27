using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class StringBinaryTranslation : IBinaryTranslation<string, Exception>
    {
        public readonly static StringBinaryTranslation Instance = new StringBinaryTranslation();

        public TryGet<string> Parse(BinaryReader reader, ulong length, bool doMasks, out Exception maskObj)
        {
            throw new NotImplementedException();
        }

        public TryGet<string> Parse(BinaryReader reader, TypeString header, byte lengthLength, bool doMasks, out Exception maskObj)
        {
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
