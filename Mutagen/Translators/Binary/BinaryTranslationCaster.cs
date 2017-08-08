using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class BinaryTranslationCaster<T, M> : IBinaryTranslation<Object, Object>
    {
        public IBinaryTranslation<T, M> Source { get; }

        public BinaryTranslationCaster(IBinaryTranslation<T, M> src)
        {
            this.Source = src;
        }

        void IBinaryTranslation<object, object>.Write(BinaryWriter writer, object item, bool doMasks, out object maskObj)
        {
            Source.Write(writer, (T)item, doMasks, out var subMaskObj);
            maskObj = subMaskObj;
        }

        TryGet<object> IBinaryTranslation<object, object>.Parse(BinaryReader reader, bool doMasks, out object maskObj)
        {
            var ret = Source.Parse(reader, doMasks, out var subMaskObj).Bubble<object>((i) => i);
            maskObj = subMaskObj;
            return ret;
        }
    }
}
