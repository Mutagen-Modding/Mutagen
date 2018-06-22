using Loqui.Internal;
using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class BinaryTranslationCaster<T> : IBinaryTranslation<Object>
    {
        public IBinaryTranslation<T> Source { get; }

        public BinaryTranslationCaster(IBinaryTranslation<T> src)
        {
            this.Source = src;
        }

        void IBinaryTranslation<object>.Write(MutagenWriter writer, object item, long length, ErrorMaskBuilder errorMask)
        {
            Source.Write(writer, (T)item, length, errorMask);
        }
        
        bool IBinaryTranslation<object>.Parse(MutagenFrame reader, out object item, ErrorMaskBuilder errorMask)
        {
            if (Source.Parse(reader, out var subObj, errorMask))
            {
                item = subObj;
                return true;
            }
            item = null;
            return false;
        }
    }
}
