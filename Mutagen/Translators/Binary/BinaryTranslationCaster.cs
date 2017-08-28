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

        TryGet<object> IBinaryTranslation<object, object>.Parse(BinaryReader reader, ulong length, bool doMasks, out object maskObj)
        {
            var ret = Source.Parse(reader, length, doMasks, out var subMaskObj).Bubble<object>((i) => i);
            maskObj = subMaskObj;
            return ret;
        }

        TryGet<object> IBinaryTranslation<object, object>.Parse(BinaryReader reader, RecordType header, byte lengthLength, bool doMasks, out object maskObj)
        {
            var ret = Source.Parse(reader, header, lengthLength, doMasks, out var subMaskObj).Bubble<object>((i) => i);
            maskObj = subMaskObj;
            return ret;
        }
    }
}
