using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class StringBinaryTranslation : IBinaryTranslation<string, Exception>
    {
        public readonly static StringBinaryTranslation Instance = new StringBinaryTranslation();

        public TryGet<string> Parse(BinaryReader reader, int length, bool doMasks, out Exception errorMask)
        {
            try
            {
                errorMask = null;
                var str = new string(reader.ReadChars(length));
                str= str.TrimEnd('\0');
                return TryGet<string>.Succeed(str);
            }
            catch (Exception ex)
            {
                if (doMasks)
                {
                    errorMask = ex;
                    return TryGet<string>.Failure;
                }
                throw;
            }
        }

        public TryGet<string> Parse(BinaryReader reader, RecordType header, byte lengthLength, bool doMasks, out Exception errorMask)
        {
            return Parse(reader, header, lengthLength: 2, doMasks: doMasks, errorMask: out errorMask);
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
